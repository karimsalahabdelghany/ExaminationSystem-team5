using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.OTP.GetValidOtpIdByUserAndPurpose;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.OTP.VerifyOtpCommand;

public record VerifyOtpCommand
(Guid UserId, string Otp , OtpPurpose Purpose) : ICommand<RequestResult<bool>>;

public class VerifyOtpCommandHandler (IUnitOfWork unitOfWork ,IMediator mediator)
    : IRequestHandler<VerifyOtpCommand, RequestResult<bool>>
{
   
    public async Task<RequestResult<bool>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var otpId = await mediator.Send(new GetValidOtpIdByUserAndPurposeQuery
                                        (request.UserId, request.Otp, request.Purpose),
                                        cancellationToken);
        if (otpId is null || otpId == Guid.Empty)
            return RequestResult<bool>.Failure(false,ResultCode.OtpNotVaild);
        var otp = new OtpCode
        {
            Id = otpId.Value,
            IsUsed = true,
            AttemptCount = 1,
        };
        var otpRepository = unitOfWork.Repository<OtpCode>();
        otpRepository.SaveInclude(otp,nameof(OtpCode.IsUsed) ,nameof(OtpCode.AttemptCount));
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return RequestResult<bool>.succeeded(true,ResultCode.OtpVerified);
    }
}
