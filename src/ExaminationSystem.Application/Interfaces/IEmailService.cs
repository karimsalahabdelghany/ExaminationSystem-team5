using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Interfaces
{
    public  interface IEmailServices
    {
        Task SendOtpEmailAsync(string email, string otp);
    }
}
