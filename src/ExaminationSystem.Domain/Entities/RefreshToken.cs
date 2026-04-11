namespace ExaminationSystem.Domain.Entities;

public class RefreshToken : BaseEntity
{
    private RefreshToken()
    {
    }

    public RefreshToken(Guid userId, string tokenHash, DateTime expiryDate, string ipAddress)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiryDate = expiryDate;
        IpAddress = ipAddress;
    }

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiryDate { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;

    public User User { get; private set; } = null!;
}

