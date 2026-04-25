using ExaminationSystem.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ExaminationSystem.Persistence.Services
{
    public class TokenServices : ITokenServies
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public TokenServices(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration; 
        }

        public async Task<string> CreateToken(AppUser appUser)
        {
            var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, appUser.Id.ToString()), 
            new Claim("user_id", appUser.Id.ToString()), 
            new Claim(JwtRegisteredClaimNames.Email, appUser.Email!),
            new Claim(JwtRegisteredClaimNames.Name, appUser.FullName!)
        };

            var roles = await _userManager.GetRolesAsync(appUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = _configuration["JwtOptions:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtOptions:Issuer"],
                audience: _configuration["JwtOptions:Audience"],
                expires: DateTime.UtcNow.AddMinutes(15), 
                claims: claims,
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }
    }
}
