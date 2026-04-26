using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.Admin.Queries;

public record GetAdminUserStatusQuery(Guid UserId) : IQuery<RequestResult<GetAdminUserStatusResponse>>;

public record GetAdminUserStatusResponse(
    Guid UserId,
    string Email,
    string FullName,
    AccountStatus Status,
    bool IsTemporarilyLocked,
    DateTime? LockedUntil,
    int FailedLoginAttempts);

public class GetAdminUserStatusQueryHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GetAdminUserStatusQuery, RequestResult<GetAdminUserStatusResponse>>
{
    public async Task<RequestResult<GetAdminUserStatusResponse>> Handle(GetAdminUserStatusQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new GetAdminUserStatusResponse(
                u.Id,
                u.Email ?? string.Empty,
                u.FullName,
                u.Status,
                u.LockedUntil.HasValue && u.LockedUntil.Value > DateTime.UtcNow,
                u.LockedUntil,
                u.FailedLoginAttempts))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return RequestResult<GetAdminUserStatusResponse>.Failure(null!, ResultCode.UserIsNotExsit);

        return RequestResult<GetAdminUserStatusResponse>.succeeded(
            user,
            ResultCode.AdminUserStatusRetrievedSuccessfully);
    }
}
