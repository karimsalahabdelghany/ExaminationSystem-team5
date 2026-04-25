using ExaminationSystem.Application.Common.Results;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.User.ProfileManagement;

public record UpdateMyProfileCommand(string FullName, string? PhoneNumber) : IRequest<RequestResult<ProfileResponse>>;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}

public class UpdateMyProfileCommandHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMyProfileCommand, RequestResult<ProfileResponse>>
{
    public async Task<RequestResult<ProfileResponse>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.TryGetUserId(out var userId))
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.InvalidCredentials);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.UserProfileNotFound);

        user.FullName = request.FullName.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = user.Id.ToString();

        await unitOfWork.BeginTransactionAsync(ct: cancellationToken);
        try
        {
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

        return RequestResult<ProfileResponse>.succeeded(response, ResultCode.UserProfileUpdatedSuccessfully);
    }
}
