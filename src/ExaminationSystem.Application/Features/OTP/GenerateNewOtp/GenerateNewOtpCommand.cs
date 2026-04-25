using ExaminationSystem.Application.Common.Helper;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace ExaminationSystem.Application.Features.OTP.GenerateNewOtp;

public record GenerateNewOtpCommand(Guid UserId, OtpPurpose Purpose)
    : ICommand<RequestResult<GenerateNewOtpResponse>>;
public record GenerateNewOtpResponse
(Guid OtpId , string otpCode);

public class GenerateNewOtpCommandHandler(IRepository<OtpCode> repository)
    : IRequestHandler<GenerateNewOtpCommand, RequestResult<GenerateNewOtpResponse>>
{
    public Task<RequestResult<GenerateNewOtpResponse>> Handle(GenerateNewOtpCommand request, CancellationToken cancellationToken)
    {
        var otp = GenerateSecureOtp();
        var hashedOtp = OtpHasher.Hash(otp);
        var expirationTime = DateTime.UtcNow.AddMinutes(10);
        var otpEntity = new OtpCode(request.UserId , hashedOtp , request.Purpose , expirationTime);
        otpEntity.Id = Guid.CreateVersion7();
        repository.Add(otpEntity);
        var result = new GenerateNewOtpResponse(otpEntity.Id ,hashedOtp);
    
        return Task.FromResult(RequestResult<GenerateNewOtpResponse>.succeeded(result,ResultCode.OtpGeneratedSuccessfully));
    }

    private static string GenerateSecureOtp()
    {

        var bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        var number = BitConverter.ToUInt32(bytes) % 1_000_000;
        return number.ToString("D6");
    }
}



