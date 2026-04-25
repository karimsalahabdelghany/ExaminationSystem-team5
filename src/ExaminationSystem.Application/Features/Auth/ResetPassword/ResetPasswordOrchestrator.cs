using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.OTP.VerifyOtpCommand;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.Auth.ResetPassword;

public record ResetPasswordOrchestrator(string Email, string Otp, string NewPassword)
    : ICommand<RequestResult<bool>>;

public class ResetPasswordOrchestratorHandler(
    UserManager<AppUser> userManager,
    IUnitOfWork unitOfWork,
    IMediator mediator)
    : IRequestHandler<ResetPasswordOrchestrator, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(ResetPasswordOrchestrator request, CancellationToken cancellationToken)
    {
        var normalizedEmail = userManager.NormalizeEmail(request.Email);
        var user = await userManager.Users
            .Where(u => u.NormalizedEmail == normalizedEmail && u.Status == AccountStatus.Active)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return RequestResult<bool>.Failure(false, ResultCode.InvalidPasswordResetOtp);

        var otpVerificationResult = await mediator.Send(
            new VerifyOtpCommand(user.Id, request.Otp, OtpPurpose.PasswordReset),
            cancellationToken);

        if (!otpVerificationResult.Success)
        {
            var code = otpVerificationResult.Code == ResultCode.OtpExpried
                ? ResultCode.PasswordResetOtpExpired
                : ResultCode.InvalidPasswordResetOtp;
            return RequestResult<bool>.Failure(false, code);
        }

        await unitOfWork.BeginTransactionAsync(ct: cancellationToken);
        try
        {
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
            if (!resetResult.Succeeded)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
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
            return RequestResult<bool>.succeeded(true, ResultCode.PasswordResetCompleted);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
