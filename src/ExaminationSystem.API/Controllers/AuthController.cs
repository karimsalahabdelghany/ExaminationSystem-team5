using ExaminationSystem.Application.Features.Auth.Login;
using ExaminationSystem.Application.Features.Auth.ChangePassword;
using ExaminationSystem.Application.Features.Auth.ForgotPassword;
using ExaminationSystem.Application.Features.Auth.RefreshAccessToken;
using ExaminationSystem.Application.Features.Auth.Register;
using ExaminationSystem.Application.Features.Auth.ResetPassword;
using ExaminationSystem.Application.Features.Auth.ResendOtpForAccountVerification;
using ExaminationSystem.Application.Features.Auth.VerifyAccount;
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
        [EnableRateLimiting("register-policy")]
        public async Task<IActionResult> RegisterAsync(RegisterOrchestrator registerCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(registerCommand, cancellationToken);
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

        [HttpPost("login")]
        [EnableRateLimiting("login-policy")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return result.Code switch
            {
                ResultCode.LoginSucceeded =>
                    Ok(ApiResponse<LoginResponse>.Success(result.Result, HttpStatusCode.OK)),

                ResultCode.InvalidCredentials =>
                    Unauthorized(ApiResponse<LoginResponse>.Failure(
                        "Invalid email or password",
                        HttpStatusCode.Unauthorized)),

                ResultCode.AccountNotActive =>
                    StatusCode(403, ApiResponse<LoginResponse>.Failure(
                        "Account is not active",
                        HttpStatusCode.Forbidden)),

                ResultCode.AccountLockedByAdmin =>
                    StatusCode(423, ApiResponse<LoginResponse>.Failure(
                        "Account is locked by admin. Please contact support.",
                        HttpStatusCode.Locked)),

                ResultCode.AccountLockedTemporarily =>
                    StatusCode(423, ApiResponse<LoginResponse>.Failure(
                        "Account locked. Try again later.",
                        HttpStatusCode.Locked)),

                _ =>
                    StatusCode(500, ApiResponse<LoginResponse>.Failure(
                        "Unexpected error",
                        HttpStatusCode.InternalServerError))
            };
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return result.Code switch
            {
                ResultCode.TokenRefreshedSuccessfully =>
                    Ok(ApiResponse<RefreshTokenResponse>.Success(result.Result, HttpStatusCode.OK)),

                ResultCode.RefreshTokenInvalid =>
                    Unauthorized(ApiResponse<RefreshTokenResponse>.Failure(
                        "Invalid refresh token",
                        HttpStatusCode.Unauthorized)),

                ResultCode.RefreshTokenExpired =>
                    Unauthorized(ApiResponse<RefreshTokenResponse>.Failure(
                        "Refresh token expired",
                        HttpStatusCode.Unauthorized)),

                ResultCode.RefreshTokenRevoked =>
                    Unauthorized(ApiResponse<RefreshTokenResponse>.Failure(
                        "Refresh token revoked",
                        HttpStatusCode.Unauthorized)),

                _ =>
                    StatusCode(500, ApiResponse<RefreshTokenResponse>.Failure(
                        "Unexpected error",
                        HttpStatusCode.InternalServerError))
            };
        }

        [HttpPost("verify-account")]
        [EnableRateLimiting("verify-otp-policy")]
        public async Task<IActionResult> VerifyAccount([FromBody] VerifyAccountOrchestrator request,
                                                  CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);

            return result.Code switch
            {
                ResultCode.AccountActivatedSuccessfully =>
                    Ok(ApiResponse<string>.Success("Account activated successfully", HttpStatusCode.OK)),

                ResultCode.OtpExpried =>
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
                    UnprocessableEntity(ApiResponse<string>.Failure(
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

        [HttpPost("forgot-password")]
        [EnableRateLimiting("forgot-password-policy")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordOrchestrator request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.Code switch
            {
                ResultCode.PasswordResetOtpSentIfAccountExists =>
                    Ok(ApiResponse<string>.Success(
                        "If this account exists, a reset OTP has been sent to the email.",
                        HttpStatusCode.OK)),

                ResultCode.FailedToSendPasswordResetOtpEmail =>
                    StatusCode(500, ApiResponse<string>.Failure(
                        "Failed to send password reset OTP email.",
                        HttpStatusCode.InternalServerError)),

                _ =>
                    BadRequest(ApiResponse<string>.Failure(
                        "Unexpected error",
                        HttpStatusCode.BadRequest))
            };
        }

        [HttpPost("reset-password")]
        [EnableRateLimiting("verify-otp-policy")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordOrchestrator request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.Code switch
            {
                ResultCode.PasswordResetCompleted =>
                    Ok(ApiResponse<string>.Success("Password reset successfully.", HttpStatusCode.OK)),

                ResultCode.InvalidPasswordResetOtp =>
                    BadRequest(ApiResponse<string>.Failure("Invalid OTP.", HttpStatusCode.BadRequest)),

                ResultCode.PasswordResetOtpExpired =>
                    BadRequest(ApiResponse<string>.Failure("OTP expired.", HttpStatusCode.BadRequest)),

                ResultCode.WeakPassword =>
                    BadRequest(ApiResponse<string>.Failure("Password does not meet policy requirements.", HttpStatusCode.BadRequest)),

                _ =>
                    BadRequest(ApiResponse<string>.Failure("Unexpected error", HttpStatusCode.BadRequest))
            };
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Code switch
            {
                ResultCode.PasswordChangedSuccessfully =>
                    Ok(ApiResponse<string>.Success("Password changed successfully.", HttpStatusCode.OK)),

                ResultCode.CurrentPasswordInvalid =>
                    BadRequest(ApiResponse<string>.Failure("Current password is invalid.", HttpStatusCode.BadRequest)),

                ResultCode.WeakPassword =>
                    BadRequest(ApiResponse<string>.Failure("Password does not meet policy requirements.", HttpStatusCode.BadRequest)),

                ResultCode.InvalidCredentials =>
                    Unauthorized(ApiResponse<string>.Failure("Unauthorized.", HttpStatusCode.Unauthorized)),

                _ =>
                    BadRequest(ApiResponse<string>.Failure("Unexpected error", HttpStatusCode.BadRequest))
            };
        }

    }
}
