namespace ExaminationSystem.Domain.Entities;

public class OtpCode : BaseEntity
{

    public Guid UserId { get; set; }
    public string CodeHash { get; set; } = string.Empty;
    public OtpPurpose Purpose { get; set; }
    public int AttemptCount { get; set; }
    public bool IsUsed { get; set; }
    public DateTime ExpiresAt { get; set; }

    public AppUser User { get; set; } = null!;
    //
    public int ResendCount { get; private set; }
    public DateTime ResendWindowStart { get; private set; }
    //

    public static OtpCode Create(Guid userId, string hashedOtp)
    {
        return new OtpCode
        {
            UserId = userId,
            CodeHash = hashedOtp,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            AttemptCount = 0,
            IsUsed = false,
            ResendCount = 0,
            ResendWindowStart = DateTime.UtcNow
        };
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    public bool IsLocked() => AttemptCount >= 5;

    public void IncrementAttempt() => AttemptCount++;
    public void MarkAsUsed() => IsUsed = true;

    public void IncrementResend()
    {
        if (DateTime.UtcNow > ResendWindowStart.AddHours(1))
        {
            ResendCount = 1;
            ResendWindowStart = DateTime.UtcNow;
        }
        else
        {
            ResendCount++;
        }
    }

    public void Refresh(string newHashedOtp)
    {
        CodeHash = newHashedOtp;
        ExpiresAt = DateTime.UtcNow.AddMinutes(10);
        AttemptCount = 0;
        IsUsed = false;
        IncrementResend();
    }

    public bool CanResend()
    {
        // Reset window if > 1 hour
        if (DateTime.UtcNow > ResendWindowStart.AddHours(1))
        {
            ResendCount = 0;
            ResendWindowStart = DateTime.UtcNow;
        }
        return ResendCount < 3;
    }



}
