namespace ExaminationSystem.Domain.Entities;

public class OtpCode : BaseEntity
{
    private OtpCode()
    {
    }

    public OtpCode(Guid userId, string codeHash, OtpPurpose purpose, DateTime expiresAt)
    {
        UserId = userId;
        CodeHash = codeHash;
        Purpose = purpose;
        ExpiresAt = expiresAt;
    }

    public Guid UserId { get; set; }
    public string CodeHash { get; set; } = string.Empty;
    public OtpPurpose Purpose { get; set; }
    public int AttemptCount { get; set; }
    public bool IsUsed { get; set; }
    public DateTime ExpiresAt { get; set; }

    public User User { get; set; } = null!;
}
