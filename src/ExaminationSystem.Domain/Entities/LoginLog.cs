namespace ExaminationSystem.Domain.Entities;

public class LoginLog : BaseEntity
{
    private LoginLog()
    {
    }

    public LoginLog(Guid userId, string ipAddress, bool success)
    {
        UserId = userId;
        IpAddress = ipAddress;
        Success = success;
    }

    public Guid UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public bool Success { get; set; }

    public User User { get; set; } = null!;
}
