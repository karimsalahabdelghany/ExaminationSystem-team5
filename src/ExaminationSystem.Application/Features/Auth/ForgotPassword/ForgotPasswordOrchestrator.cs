using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.OTP.GenerateNewOtp;
using ExaminationSystem.Application.Services.EmailService;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace ExaminationSystem.Application.Features.Auth.ForgotPassword;

public record ForgotPasswordOrchestrator(string Email) : ICommand<RequestResult<bool>>;

public class ForgotPasswordOrchestratorHandler(
    UserManager<AppUser> userManager,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IMediator mediator,
    ILogger<ForgotPasswordOrchestratorHandler> logger)
    : IRequestHandler<ForgotPasswordOrchestrator, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(ForgotPasswordOrchestrator request, CancellationToken cancellationToken)
    {
        var normalizedEmail = userManager.NormalizeEmail(request.Email);
        var user = await userManager.Users
            .Where(u => u.NormalizedEmail == normalizedEmail && u.Status == AccountStatus.Active)
            .FirstOrDefaultAsync(cancellationToken);

        // Prevent account enumeration by returning success in all cases.
        if (user is null)
            return RequestResult<bool>.succeeded(true, ResultCode.PasswordResetOtpSentIfAccountExists);

        await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var otpResult = await mediator.Send(
                new GenerateNewOtpCommand(user.Id, OtpPurpose.PasswordReset),
                cancellationToken);

            if (!otpResult.Success)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<bool>.Failure(false, otpResult.Code);
            }

            var sendEmailResult = await emailService.SendAsync(new EmailRequest
            {
                To = user.Email!,
                Subject = "Password reset OTP",
                Body = $"Your password reset OTP is: {otpResult.Result?.otpCode ?? string.Empty}. It will expire in 10 minutes."
            }, cancellationToken);

            if (!sendEmailResult.IsSuccess)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                logger.LogError("Failed to send password-reset OTP email to {Email}", request.Email);
                return RequestResult<bool>.Failure(false, ResultCode.FailedToSendPasswordResetOtpEmail);
            }

            await unitOfWork.CommitAsync(cancellationToken);
            return RequestResult<bool>.succeeded(true, ResultCode.PasswordResetOtpSentIfAccountExists);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
