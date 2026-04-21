using ExaminationSystem.Application.Common.Results;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.OTP
{
    public record VerifyOtpCommand(string Email, string Otp) : IRequest<RequestResult<string>>;

    class VerifyOtpCommandHanlder : IRequestHandler<VerifyOtpCommand, RequestResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOTPRepository _oTPRepository;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public VerifyOtpCommandHanlder(UserManager<AppUser> userManager , IOTPRepository oTPRepository , IPasswordHasher<AppUser> passwordHasher)
        {
            _userManager = userManager;
            _oTPRepository = oTPRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<RequestResult<string>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var user =  await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return RequestResult<string>.Failure("User.NotFound" ,ResultCode.UserIsNotExsit );

            var OtpCode =  await _oTPRepository.GetActiveByUserIdAsync(user.Id, cancellationToken); 
            if (OtpCode is null) 
                return RequestResult<string>.Failure( "No active OTP found.", ResultCode.NoActiveOTPFound) ; 

            if (OtpCode.IsLocked())
                return RequestResult<string>.Failure( "Account locked due to too many attempts.",ResultCode.AccountLocked );

            if (OtpCode.IsExpired())
                return RequestResult<string>.Failure( "OTP expired.", ResultCode.OtpExpried);

            var dummyUser = new AppUser();
            var verifyResult = _passwordHasher.VerifyHashedPassword(dummyUser, OtpCode.CodeHash, request.Otp);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                OtpCode.IncrementAttempt();
                await _oTPRepository.UpdateAsync(OtpCode, cancellationToken);
                return RequestResult<string>.Failure(   $"Invalid OTP. Attempts: {OtpCode.AttemptCount}/5", ResultCode.OtpNotVaild);
            }

            OtpCode.MarkAsUsed();
            await _oTPRepository.UpdateAsync(OtpCode, cancellationToken);

            user.EmailConfirmed = true; // Microsoft Identity: active = EmailConfirmed
            await _userManager.UpdateAsync(user);

            return RequestResult<string>.succeeded("Account activated successfully." , ResultCode.AccountActivatedSuccessfully);



        }
    }

}
