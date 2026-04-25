using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.OTP.GenerateNewOtp;
using ExaminationSystem.Application.Services.EmailService;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace ExaminationSystem.Application.Features.Auth.ResendOtpForAccountVerification;

public record ResendOtpForAccountVerificationOrchestrator
(string UserEmail) : ICommand<RequestResult<bool>>;

public class ResendOtpForAccountVerificationOrchestratorHandler(UserManager<AppUser> userManager ,IUnitOfWork unitOfWork
    ,ILogger<ResendOtpForAccountVerificationOrchestratorHandler> logger , IEmailService emailService,IMediator mediator)
    : IRequestHandler<ResendOtpForAccountVerificationOrchestrator, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(ResendOtpForAccountVerificationOrchestrator request, CancellationToken cancellationToken)
    {
        var normalizedEmail = userManager.NormalizeEmail(request.UserEmail);
        var userId = await userManager.Users
                                                              .Where
                                                              (
                                                                  u => u.NormalizedEmail == normalizedEmail
                                                                    && u.Status == AccountStatus.PendingVerification
                                                              ).Select(u => u.Id)
                                                              .FirstOrDefaultAsync(cancellationToken);
        if(userId == Guid.Empty)
        {
            logger.LogWarning("User with Email :{userEmail} not Exist or already Verfiy his account" ,request.UserEmail);
            return RequestResult<bool>.Failure(false, ResultCode.UserEmailIsNotExistOrAccountIsNotInPendingStatus);
        }    
        await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        var generateOtpResult = await mediator.Send(new GenerateNewOtpCommand(userId ,OtpPurpose.EmailConfirmation) ,cancellationToken);
        if(!generateOtpResult.Success)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            logger.LogError("Failed to Generate OTP for User with Email :{userEmail} , Error Code :{errorCode}" ,request.UserEmail ,generateOtpResult.Code);
            return RequestResult<bool>.Failure(false, generateOtpResult.Code);
        }

        var sendEmailResult = await emailService.SendAsync(new EmailRequest {
            To = request.UserEmail,
            Subject = "Your OTP Code for Account Verification",
            Body = $"Your OTP code is: {generateOtpResult?.Result?.OtpId}. It will expire in 10 minutes."
        }, cancellationToken);

        if(!sendEmailResult.IsSuccess)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            logger.LogError("Failed to send OTP email to User with Email :{userEmail}" ,request.UserEmail);
            return RequestResult<bool>.Failure(false, ResultCode.FailedToSendRegisterEmail);
        }
        try
        {
            await unitOfWork.CommitAsync(cancellationToken);
            return RequestResult<bool>.succeeded(true, ResultCode.OTPResentSuccessfully);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            logger.LogError("Failed to send OTP email to User with Email :{userEmail}", request.UserEmail);
            return RequestResult<bool>.Failure(false, ResultCode.FailedToSendRegisterEmail);

        }
    }
}




