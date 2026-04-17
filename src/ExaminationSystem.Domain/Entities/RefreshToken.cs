namespace ExaminationSystem.Domain.Entities;

public class RefreshToken : BaseEntity
{
    private RefreshToken()
    {
    }

    public RefreshToken(Guid userId, string tokenHash, DateTime expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public bool IsRevoked { get; set; }
    public DateTime ExpiresAt { get; set; }

    public AppUser User { get; set; } = null!;
}
