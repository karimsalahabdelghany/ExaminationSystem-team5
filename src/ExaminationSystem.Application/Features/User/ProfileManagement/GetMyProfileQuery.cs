using ExaminationSystem.Application.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.User.ProfileManagement;

public record GetMyProfileQuery : IRequest<RequestResult<ProfileResponse>>;

public class GetMyProfileQueryHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser)
    : IRequestHandler<GetMyProfileQuery, RequestResult<ProfileResponse>>
{
    public async Task<RequestResult<ProfileResponse>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.TryGetUserId(out var userId))
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.InvalidCredentials);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return RequestResult<ProfileResponse>.Failure(null, ResultCode.UserProfileNotFound);

        var response = new ProfileResponse(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            user.PendingEmail,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.Status.ToString());

        return RequestResult<ProfileResponse>.succeeded(response, ResultCode.UserProfileRetrievedSuccessfully);
    }
}
