using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.OTP.GenerateNewOtp;
using ExaminationSystem.Application.Services.EmailService;
using ExaminationSystem.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.User.ProfileManagement;

public record RequestEmailChangeOtpCommand(string NewEmail) : IRequest<RequestResult<bool>>;

public class RequestEmailChangeOtpCommandValidator : AbstractValidator<RequestEmailChangeOtpCommand>
{
    public RequestEmailChangeOtpCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}

public class RequestEmailChangeOtpCommandHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser,
    IEmailService emailService,
    IMediator mediator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RequestEmailChangeOtpCommand, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(RequestEmailChangeOtpCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.TryGetUserId(out var userId))
            return RequestResult<bool>.Failure(false, ResultCode.InvalidCredentials);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return RequestResult<bool>.Failure(false, ResultCode.UserProfileNotFound);

        var normalizedNewEmail = userManager.NormalizeEmail(request.NewEmail);
        var currentNormalizedEmail = userManager.NormalizeEmail(user.Email ?? string.Empty);
        if (normalizedNewEmail == currentNormalizedEmail)
            return RequestResult<bool>.Failure(false, ResultCode.ValidationError);

        var emailInUse = await userManager.Users
            .AnyAsync(u => u.NormalizedEmail == normalizedNewEmail && u.Id != user.Id, cancellationToken);
        if (emailInUse)
            return RequestResult<bool>.Failure(false, ResultCode.EmailAlreadyInUse);

        await unitOfWork.BeginTransactionAsync(ct: cancellationToken);
        try
        {
            var otpResult = await mediator.Send(new GenerateNewOtpCommand(user.Id, OtpPurpose.EmailChange), cancellationToken);
            if (!otpResult.Success)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<bool>.Failure(false, otpResult.Code);
            }

            user.PendingEmail = request.NewEmail.Trim();
            user.PendingEmailRequestedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = user.Id.ToString();

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<bool>.Failure(false, ResultCode.ValidationError);
            }

            var sendEmailResult = await emailService.SendAsync(new EmailRequest
            {
                To = user.PendingEmail,
                Subject = "Email change OTP",
                Body = $"Your OTP to confirm email change is: {otpResult.Result?.otpCode ?? string.Empty}. It expires in 10 minutes."
            }, cancellationToken);

            if (!sendEmailResult.IsSuccess)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<bool>.Failure(false, ResultCode.FailedToSendPasswordResetOtpEmail);
            }

            await unitOfWork.CommitAsync(cancellationToken);
            return RequestResult<bool>.succeeded(true, ResultCode.EmailChangeOtpSentSuccessfully);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
