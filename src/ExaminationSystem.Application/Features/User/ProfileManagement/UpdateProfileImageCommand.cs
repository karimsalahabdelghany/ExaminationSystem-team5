using ExaminationSystem.Application.Common.Results;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.User.ProfileManagement;

public record UpdateProfileImageCommand(byte[] FileBytes, string FileExtension) : IRequest<RequestResult<ProfileResponse>>;

public class UpdateProfileImageCommandValidator : AbstractValidator<UpdateProfileImageCommand>
{
    private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public UpdateProfileImageCommandValidator()
    {
        RuleFor(x => x.FileBytes)
            .NotNull().WithMessage("Image is required.")
            .Must(bytes => bytes.Length > 0).WithMessage("Image cannot be empty.")
            .Must(bytes => bytes.Length <= 5 * 1024 * 1024).WithMessage("Image size must not exceed 5MB.");

        RuleFor(x => x.FileExtension)
            .NotEmpty().WithMessage("Image extension is required.")
            .Must(ext => AllowedExtensions.Contains(ext.ToLowerInvariant()))
            .WithMessage("Only jpg, jpeg, png, and webp are allowed.");
    }
}

public class UpdateProfileImageCommandHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser,
    IProfileImageService profileImageService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProfileImageCommand, RequestResult<ProfileResponse>>
{
    public async Task<RequestResult<ProfileResponse>> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.TryGetUserId(out var userId))
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.InvalidCredentials);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.UserProfileNotFound);

        var oldImageUrl = user.ProfileImageUrl;
        var newImageUrl = await profileImageService.SaveAsync(user.Id, request.FileBytes, request.FileExtension, cancellationToken);

        user.ProfileImageUrl = newImageUrl;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = user.Id.ToString();

        await unitOfWork.BeginTransactionAsync(ct: cancellationToken);
        try
        {
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                await profileImageService.DeleteAsync(newImageUrl, cancellationToken);
                return RequestResult<ProfileResponse>.Failure(null, ResultCode.ValidationError);
            }

            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            await profileImageService.DeleteAsync(newImageUrl, cancellationToken);
            throw;
        }

        await profileImageService.DeleteAsync(oldImageUrl, cancellationToken);

        var response = new ProfileResponse(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            user.PendingEmail,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.Status.ToString());

        return RequestResult<ProfileResponse>.succeeded(response, ResultCode.ProfileImageUpdatedSuccessfully);
    }
}
