using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.Admin.Commands.SetUserLockState;

public record SetUserLockStateCommand(Guid UserId, bool IsLocked) : ICommand<RequestResult<bool>>;

public class SetUserLockStateCommandHandler(
    UserManager<AppUser> userManager,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SetUserLockStateCommand, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(SetUserLockStateCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(ct: cancellationToken);
        try
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<bool>.Failure(false, ResultCode.UserIsNotExsit);
            }

            if (request.IsLocked)
            {
                user.Status = AccountStatus.Locked;
                user.LockedUntil = null;
                user.FailedLoginAttempts = 0;

                // Lock through Identity system as well, to enforce auth checks consistently.
                var lockoutResult = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                if (!lockoutResult.Succeeded)
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                    return RequestResult<bool>.Failure(false, ResultCode.ValidationError);
                }

                var lockResult = await userManager.UpdateAsync(user);
                if (!lockResult.Succeeded)
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                    return RequestResult<bool>.Failure(false, ResultCode.ValidationError);
                }

                var activeRefreshTokens = await unitOfWork.Repository<RefreshToken>()
                    .GetAll(t => t.UserId == user.Id && !t.IsRevoked)
                    .ToListAsync(cancellationToken);

                foreach (var refreshToken in activeRefreshTokens)
                {
                    refreshToken.IsRevoked = true;
                    unitOfWork.Repository<RefreshToken>().Update(refreshToken);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);
                return RequestResult<bool>.succeeded(true, ResultCode.AccountLockedByAdmin);
            }

            user.Status = AccountStatus.Active;
            user.LockedUntil = null;
            user.FailedLoginAttempts = 0;

            var unlockLockoutResult = await userManager.SetLockoutEndDateAsync(user, null);
            if (!unlockLockoutResult.Succeeded)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<bool>.Failure(false, ResultCode.ValidationError);
            }

            var unlockResult = await userManager.UpdateAsync(user);
            if (!unlockResult.Succeeded)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<bool>.Failure(false, ResultCode.ValidationError);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            return RequestResult<bool>.succeeded(true, ResultCode.AccountUnlockedByAdmin);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
