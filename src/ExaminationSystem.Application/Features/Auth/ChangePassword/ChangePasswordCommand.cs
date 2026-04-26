using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.Auth.ChangePassword;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword)
    : ICommand<RequestResult<bool>>;

public class ChangePasswordCommandHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangePasswordCommand, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.TryGetUserId(out var userId))
            return RequestResult<bool>.Failure(false, ResultCode.InvalidCredentials);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.Status != AccountStatus.Active)
            return RequestResult<bool>.Failure(false, ResultCode.InvalidCredentials);

        await unitOfWork.BeginTransactionAsync(ct: cancellationToken);
        try
        {
            var changeResult = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!changeResult.Succeeded)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                if (changeResult.Errors.Any(e => e.Code == "PasswordMismatch"))
                    return RequestResult<bool>.Failure(false, ResultCode.CurrentPasswordInvalid);

                return RequestResult<bool>.Failure(false, ResultCode.WeakPassword);
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
            return RequestResult<bool>.succeeded(true, ResultCode.PasswordChangedSuccessfully);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
