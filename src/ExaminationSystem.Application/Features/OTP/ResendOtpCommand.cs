using ExaminationSystem.Application.Common.Results;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ExaminationSystem.Application.Features.OTP
{
    public record ResendOtpCommand(string Email) : IRequest<RequestResult<string>>;

    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, RequestResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOTPRepository _oTPRepository;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IEmailService _emailService;
        public ResendOtpCommandHandler(UserManager<AppUser> userManager, IOTPRepository oTPRepository, IPasswordHasher<AppUser> passwordHasher , IEmailService emailService)
        {
            _userManager = userManager;
            _oTPRepository = oTPRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService; 
        }
        public async Task<RequestResult<string>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return RequestResult<string>.Failure("User Is not Found", ResultCode.UserIsNotExsit);

            var exsitOtp = await _oTPRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);
            if (exsitOtp is not null && !exsitOtp.CanResend())
                return RequestResult<string>.Failure("Can not resend new opt", ResultCode.ResendLimitExceeded);

            var rawOtp = GenerateSecureOtp();
            var dummyUser = new AppUser();
            var hashedOtp = _passwordHasher.HashPassword(dummyUser, rawOtp);

            if (exsitOtp is null)
            {
                var newRecord = OtpCode.Create(user.Id, hashedOtp);
                await _oTPRepository.AddAsync(newRecord, cancellationToken);
            }
            else
            {
                exsitOtp.Refresh(hashedOtp);
                await _oTPRepository.UpdateAsync(exsitOtp, cancellationToken);
            }
            //await _emailService.SendOtpEmailAsync(user.Email!, rawOtp);

            return RequestResult<string>.succeeded("OTP resent successfully." , ResultCode.OTPResentSuccessfully);

        }
        private static string GenerateSecureOtp()
        {
            
            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            var number = BitConverter.ToUInt32(bytes) % 1_000_000;
            return number.ToString("D6");
        }
    }
}



   

    
