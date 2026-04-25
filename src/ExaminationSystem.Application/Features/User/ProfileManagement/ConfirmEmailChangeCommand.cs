using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.OTP.VerifyOtpCommand;
using ExaminationSystem.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.User.ProfileManagement;

public record ConfirmEmailChangeCommand(string Otp) : IRequest<RequestResult<ProfileResponse>>;

public class ConfirmEmailChangeCommandValidator : AbstractValidator<ConfirmEmailChangeCommand>
{
    public ConfirmEmailChangeCommandValidator()
    {
        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP is required.")
            .Length(6).WithMessage("OTP must be 6 digits.");
    }
}

public class ConfirmEmailChangeCommandHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IMediator mediator)
    : IRequestHandler<ConfirmEmailChangeCommand, RequestResult<ProfileResponse>>
{
    public async Task<RequestResult<ProfileResponse>> Handle(ConfirmEmailChangeCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.TryGetUserId(out var userId))
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.InvalidCredentials);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.UserProfileNotFound);

        if (string.IsNullOrWhiteSpace(user.PendingEmail))
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.PendingEmailChangeNotFound);

        var normalizedPendingEmail = userManager.NormalizeEmail(user.PendingEmail);
        var emailInUse = await userManager.Users
            .AnyAsync(u => u.NormalizedEmail == normalizedPendingEmail && u.Id != user.Id, cancellationToken);
        if (emailInUse)
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.EmailAlreadyInUse);

        var otpVerification = await mediator.Send(
            new VerifyOtpCommand(user.Id, request.Otp, OtpPurpose.EmailChange),
            cancellationToken);

        if (!otpVerification.Success)
            return RequestResult<ProfileResponse>.Failure(null, otpVerification.Code);

        await unitOfWork.BeginTransactionAsync(ct: cancellationToken);
        try
        {
            user.Email = user.PendingEmail.Trim();
            user.NormalizedEmail = userManager.NormalizeEmail(user.Email);
            user.UserName = user.Email;
            user.NormalizedUserName = userManager.NormalizeName(user.Email);
            user.EmailConfirmed = true;
            user.PendingEmail = null;
            user.PendingEmailRequestedAt = null;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = user.Id.ToString();

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<ProfileResponse>.Failure(null, ResultCode.ValidationError);
            }

            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }

        var response = new ProfileResponse(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            user.PendingEmail,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.Status.ToString());

        return RequestResult<ProfileResponse>.succeeded(response, ResultCode.EmailChangedSuccessfully);
    }
}
