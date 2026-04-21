using ExaminationSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ExaminationSystem.Persistence.Services
{
    public class EmailServices  :IEmailServices
    {
        public async Task SendOtpEmailAsync(string email, string otp)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(
                    "your-email@gmail.com",
                    "your-app-password"
                ),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress("your-email@gmail.com"),
                Subject = "Your OTP Code",
                Body = $"Your OTP is: {otp}",
                IsBodyHtml = false
            };

            message.To.Add(email);

            await smtpClient.SendMailAsync(message);
        }
    }
}
