using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ExaminationSystem.Application.Features.Register
{
    public record RegisterCommand(string FullName, string Email, string Password) : IRequest<RequestResult<RegisterResponse>>;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RequestResult<RegisterResponse>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailServices _emailService;
        private readonly IOTPRepository _oTPRepository;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public RegisterCommandHandler(UserManager<AppUser> userManager , IEmailServices emailService, IOTPRepository oTPRepository, IPasswordHasher<AppUser> passwordHasher)
        {
            _userManager = userManager;
            _emailService = emailService; 
            _oTPRepository = oTPRepository;
            _passwordHasher = passwordHasher;   
        }

        public   async Task<RequestResult<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

          
            var email= _userManager.NormalizeEmail(request.Email);  
                var isExistEmail = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == email , cancellationToken);


            
           if (isExistEmail)
            {
                return RequestResult<RegisterResponse>.Failure(null , ResultCode.UserIsAlredyExist); 
            }

            var newUser = request.Adapt<AppUser>();
            newUser.Id = Guid.CreateVersion7();
            newUser.Status = AccountStatus.PendingVerification;

            var createUser = await _userManager.CreateAsync(newUser , request.Password);


            if (createUser.Succeeded)
            {
                // 1. Generate OTP
                var otp = GenerateSecureOtp();

                // 2. Hash OTP
                var hashedOtp = _passwordHasher.HashPassword(newUser, otp);

                // 3. Create OTP Entity
                var otpEntity = OtpCode.Create(
                    newUser.Id,
                    hashedOtp
                  
                );

                // 4. Save OTP
                await _oTPRepository.AddAsync(otpEntity, cancellationToken);

                // 5. Send Email
                await _emailService.SendOtpEmailAsync(newUser.Email!, otp);

                return RequestResult<RegisterResponse>.succeeded(
                    new RegisterResponse(newUser.Id),
                    ResultCode.UserCreateSuccesfully
                );
            }

            else
            {
                var errors = createUser.Errors.Select(e => e.Description).ToList();

                return RequestResult<RegisterResponse>
                    .Failure(new RegisterResponse(newUser.Id, errors), ResultCode.UserCreateFilad);
            }







        }

        #region Herpler 
       

        private static string GenerateSecureOtp()
        {

            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            var number = BitConverter.ToUInt32(bytes) % 1_000_000;
            return number.ToString("D6");
        } 
        #endregion
    }

}
