using ExaminationSystem.Application.Common.Helper;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.OTP.GetValidOtpIdByUserAndPurpose;

public record GetValidOtpIdByUserAndPurposeQuery
(Guid UserId, string Otp, OtpPurpose Purpose) : IRequest<Guid?>;

public class GetValidOtpIdByUserAndPurposeQueryHandler(IRepository<OtpCode> otpRepository)
    : IRequestHandler<GetValidOtpIdByUserAndPurposeQuery, Guid?>
{
    public async Task<Guid?> Handle(GetValidOtpIdByUserAndPurposeQuery request, CancellationToken cancellationToken)
    {
        var hashedOtp = OtpHasher.Hash(request.Otp);
        var otpId = await otpRepository.GetAll(otp => otp.UserId == request.UserId
                                                  && otp.CodeHash == hashedOtp
                                                  && otp.Purpose == request.Purpose
                                                  && otp.ExpiresAt >= DateTime.UtcNow
                                                  && !otp.IsUsed)
                                            .Select(otp => otp.Id)
                                            .FirstOrDefaultAsync(cancellationToken);
        return otpId;
    }
}


