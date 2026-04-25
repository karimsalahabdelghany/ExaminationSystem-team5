
using ExaminationSystem.Application.Common.Services.EmailService;
using ExaminationSystem.Application.Services.EmailService;

namespace ExaminationSystem.Application.Interfaces;

 public  interface IEmailService
{
    //Task SendOtpEmailAsync(string email, string otp);
    Task<EmailResult> SendAsync(EmailRequest  emailRequest , CancellationToken cancellationToken);
 }

