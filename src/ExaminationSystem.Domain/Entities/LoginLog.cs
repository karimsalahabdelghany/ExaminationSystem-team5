namespace ExaminationSystem.Domain.Entities;

public class LoginLog : BaseEntity
{
    private LoginLog()
    {
    }

    public LoginLog(Guid userId, string ipAddress, string userAgent, bool success)
    {
        UserId = userId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Success = success;
    }

    public Guid UserId { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;
    public bool Success { get; private set; }

    public User User { get; private set; } = null!;
}

