using ExaminationSystem.Application.Common.Services.EmailService;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Application.Services.EmailService;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;


namespace ExaminationSystem.Persistence.Services
{
    // Services/EmailService.cs
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }
        //public async Task SendOtpEmailAsync(string email, string otp)
        //{
        //    var smtpClient = new SmtpClient("smtp.gmail.com")
        //    {
        //        Port = 587,
        //        Credentials = new NetworkCredential(
        //            "your-email@gmail.com",
        //            "your-app-password"
        //        ),
        //        EnableSsl = true
        //    };

        //    var message = new MailMessage
        //    {
        //        From = new MailAddress("your-email@gmail.com"),
        //        Subject = "Your OTP Code",
        //        Body = $"Your OTP is: {otp}",
        //        IsBodyHtml = false
        //    };

        //    message.To.Add(email);

        //    await smtpClient.SendMailAsync(message);
        //}
        public async Task<EmailResult> SendAsync(EmailRequest request, CancellationToken ct)
        {
            try
            {
                var message = BuildMessage(request);

                using var client = new SmtpClient();

                await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort,
                    SecureSocketOptions.StartTls, ct);

                await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);

                await client.SendAsync(message, ct);

                await client.DisconnectAsync(true, ct);

                _logger.LogInformation("✅ Email sent successfully to {Recipient}", request.To);

                return EmailResult.Success();
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError(ex, "❌ SMTP authentication failed");
                return EmailResult.Fail("SMTP authentication failed. Check your credentials.");
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError(ex, "❌ SMTP command error: {StatusCode}", ex.StatusCode);
                return EmailResult.Fail($"SMTP error: {ex.Message}");
            }
            catch (SmtpProtocolException ex)
            {
                _logger.LogError(ex, "❌ SMTP protocol error");
                return EmailResult.Fail("SMTP protocol error. Please try again.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("⚠️ Email sending was cancelled");
                return EmailResult.Fail("Email sending was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error sending email to {Recipient}", request.To);
                return EmailResult.Fail($"Unexpected error: {ex.Message}");
            }
        }

        private MimeMessage BuildMessage(EmailRequest request)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(request.To));

            foreach (var cc in request.Cc)
                message.Cc.Add(MailboxAddress.Parse(cc));

            message.Subject = request.Subject;

            var builder = new BodyBuilder();
            if (request.IsHtml)
                builder.HtmlBody = request.Body;
            else
                builder.TextBody = request.Body;

            foreach (var (fileName, content, contentType) in request.Attachments)
                builder.Attachments.Add(fileName, content, ContentType.Parse(contentType));

            message.Body = builder.ToMessageBody();
            return message;
        }
    }
}
