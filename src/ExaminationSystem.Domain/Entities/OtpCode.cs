using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Domain.Entities;

public class OtpCode : BaseEntity
{
    private OtpCode()
    {
    }

    public OtpCode(Guid userId, string code, OtpPurpose purpose, DateTime expiryDate)
    {
        UserId = userId;
        Code = code;
        Purpose = purpose;
        ExpiryDate = expiryDate;
    }

    public Guid UserId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public OtpPurpose Purpose { get; private set; }
    public DateTime ExpiryDate { get; private set; }

    public User User { get; private set; } = null!;
}

