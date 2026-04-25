using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.OTP.VerifyOtpCommand;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.Auth.VerifyAccount;

public record VerifyAccountOrchestrator
(string Email, string Otp) : IQuery<RequestResult<bool>>;

public class VerifyAccountOrchestratorHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMediator mediator)
    : IRequestHandler<VerifyAccountOrchestrator, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(VerifyAccountOrchestrator request, CancellationToken cancellationToken)
    {
        var normalizedEmail = userManager.NormalizeEmail(request.Email);
        var user = await userManager.Users.Where(u => u.NormalizedEmail == normalizedEmail
                                                 && u.Status == AccountStatus.PendingVerification)
                                          .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
            return RequestResult<bool>.Failure(false, ResultCode.UserIsNotExsit);

        var otpVerificationResult = await mediator.Send(new VerifyOtpCommand(user.Id, request.Otp, OtpPurpose.AccountVerification), cancellationToken);
        if (!otpVerificationResult.Success)
            return RequestResult<bool>.Failure(false, otpVerificationResult.Code);
        user.EmailConfirmed = true;
        user.Status = AccountStatus.Active;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return RequestResult<bool>.Failure(false, ResultCode.CanNotVerifyAccount);
        return RequestResult<bool>.succeeded(true, ResultCode.AccountActivatedSuccessfully);
    }
}
