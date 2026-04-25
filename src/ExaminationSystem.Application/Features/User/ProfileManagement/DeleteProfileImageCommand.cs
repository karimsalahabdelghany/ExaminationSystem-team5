using ExaminationSystem.Application.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.User.ProfileManagement;

public record DeleteProfileImageCommand : IRequest<RequestResult<ProfileResponse>>;

public class DeleteProfileImageCommandHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser,
    IProfileImageService profileImageService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProfileImageCommand, RequestResult<ProfileResponse>>
{
    public async Task<RequestResult<ProfileResponse>> Handle(DeleteProfileImageCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.TryGetUserId(out var userId))
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.InvalidCredentials);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.UserProfileNotFound);

        var oldImageUrl = user.ProfileImageUrl;
        user.ProfileImageUrl = null;
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

        await profileImageService.DeleteAsync(oldImageUrl, cancellationToken);

        var response = new ProfileResponse(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            user.PendingEmail,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.Status.ToString());

        return RequestResult<ProfileResponse>.succeeded(response, ResultCode.ProfileImageRemovedSuccessfully);
    }
}
