using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Register;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Auth.Login
{

    public record LoginCommand(string password, string email) : IRequest<RequestResult<string>>;
    public class LoginCommandHandler : IRequestHandler<LoginCommand, RequestResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenServies _tokenServies;

        public LoginCommandHandler(UserManager<AppUser> userManager , ITokenServies tokenServies)
        {
           _userManager = userManager;
           _tokenServies = tokenServies;
        }
        public async Task<RequestResult<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.email);
            if (user == null) {
              
                 return   RequestResult<string>.Failure(null, ResultCode.Invalidcredentials); 
            }

           var isPasswordVaild =  await _userManager.CheckPasswordAsync( user , request.password );
            if (!isPasswordVaild)
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                }

                await _userManager.UpdateAsync(user);

            }
                return RequestResult<string>.Failure(null, ResultCode.Invalidcredentials);

            if (user.Status != AccountStatus.Active)
                return RequestResult<string>.Failure(null, ResultCode.AccountNotverified);


            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;

          
            var accessToken = await _tokenServies.CreateToken(user);
            var refreshTokenValue = _tokenServies.GenerateRefreshToken();

            var refreshToken = new RefreshToken(
                   user.Id,
                tokenHashString,
                 DateTime.UtcNow.AddDays(7)
                 );

            // تضيفه
            user.AddRefreshToken(refreshToken);

            // ✅ log login
            var loginLog = new LoginLog
            {
                UserId = user.Id,
                LoginTime = DateTime.UtcNow,
                IpAddress = "IP_FROM_REQUEST"
            };

            user.AddLoginLog(loginLog);

            await _userManager.UpdateAsync(user);


        }
    }
}
