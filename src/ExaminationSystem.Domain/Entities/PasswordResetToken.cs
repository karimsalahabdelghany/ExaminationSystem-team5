namespace ExaminationSystem.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    private PasswordResetToken()
    {
    }

    public PasswordResetToken(Guid userId, string tokenHash, DateTime expiryDate)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiryDate = expiryDate;
    }

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiryDate { get; private set; }

    public User User { get; private set; } = null!;
}

