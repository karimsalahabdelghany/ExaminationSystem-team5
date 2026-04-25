using ExaminationSystem.Application.Features.Auth.Register;
using ExaminationSystem.Application.Features.Auth.ResendOtpForAccountVerification;
using ExaminationSystem.Application.Features.OTP;

using Microsoft.AspNetCore.RateLimiting;

namespace ExaminationSystem.API.Controllers
{
    
    public class AuthController : BaseController
    {
       
        public AuthController(IMediator mediator) : base(mediator)
        {
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync (RegisterOrchestrator registerCommand , CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(registerCommand , cancellationToken);
            return result.Code switch
            {
                ResultCode.UserCreateSuccesfully =>
                    Ok(ApiResponse<RegisterResponse>.Success(result.Result, HttpStatusCode.OK)),

                ResultCode.UserIsAlredyExist =>
                    BadRequest(ApiResponse<RegisterResponse>.Failure(
                        "User already exists",
                        HttpStatusCode.BadRequest)),

                ResultCode.UserCreateFilad =>
                    BadRequest(ApiResponse<RegisterResponse>.Failure(
                        "User creation failed",
                        HttpStatusCode.BadRequest)),

                _ =>
                    StatusCode(500, ApiResponse<RegisterResponse>.Failure(
                        "Unexpected error",
                        HttpStatusCode.InternalServerError))
            };


        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(
    [FromBody] VerifyOtpCommand request,
    CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);

            return result.Code switch
            {
                ResultCode.AccountActivatedSuccessfully =>
                    Ok(ApiResponse<string>.Success(result.Result, HttpStatusCode.OK)),

                ResultCode.OtpExpried=>
                    BadRequest(ApiResponse<string>.Failure("OTP expired", HttpStatusCode.BadRequest)),

                ResultCode.OtpNotVaild =>
                    BadRequest(ApiResponse<string>.Failure("Invalid OTP", HttpStatusCode.BadRequest)),

                ResultCode.AccountLocked =>
                    StatusCode(423, ApiResponse<string>.Failure("Account locked", HttpStatusCode.Locked)),

                ResultCode.NoActiveOTPFound =>
                    NotFound(ApiResponse<string>.Failure("No active OTP found", HttpStatusCode.NotFound)),

                _ =>
                    BadRequest(ApiResponse<string>.Failure("Unexpected error", HttpStatusCode.BadRequest))
            };
        }

        [HttpPost("resend-otp")]
        [EnableRateLimiting("resend-otp-policy")]
        public async Task<IActionResult> ResendOtp(
            [FromBody] ResendOtpForAccountVerificationOrchestrator request,
            CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);

            return result.Code switch
            {
                ResultCode.OTPResentSuccessfully =>
                    Ok(ApiResponse<string>.Success("OTP sent successfully", HttpStatusCode.OK)),

                ResultCode.ResendLimitExceeded =>
                    UnprocessableEntity( ApiResponse<string>.Failure(
                        "Resend limit exceeded",
                        HttpStatusCode.TooManyRequests)),

                ResultCode.UserEmailIsNotExistOrAccountIsNotInPendingStatus =>
                    NotFound(ApiResponse<string>.Failure(
                        "User not found or account is not in pending status",
                        HttpStatusCode.NotFound)),

                ResultCode.NoActiveOTPFound =>
                    NotFound(ApiResponse<string>.Failure(
                        "No active OTP found",
                        HttpStatusCode.NotFound)),

                _ =>
                    BadRequest(ApiResponse<string>.Failure(
                        "Unexpected error",
                        HttpStatusCode.BadRequest))
            };
        }

    }
}
