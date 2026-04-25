
namespace ExaminationSystem.Application.Services.EmailService;

public class EmailRequest
{
    public required string To { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public bool IsHtml { get; set; } = false;
    public List<string> Cc { get; set; } = [];
    public List<(string FileName, Stream Content, string ContentType)> Attachments { get; set; } = [];
}
