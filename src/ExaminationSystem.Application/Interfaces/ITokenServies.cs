using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Interfaces
{
    public  interface ITokenServies
    {
        public Task<string> CreateToken(AppUser appUser);
        public string GenerateRefreshToken();
    }
}
