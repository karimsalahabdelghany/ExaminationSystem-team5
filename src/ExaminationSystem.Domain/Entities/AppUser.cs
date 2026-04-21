using ExaminationSystem.Domain.Interfaces;

namespace ExaminationSystem.Domain.Entities;

public class AppUser : IdentityUser<Guid>, IBaseEntity
{
    private readonly List<OtpCode> _otpCodes = [];
    private readonly List<PasswordResetToken> _passwordResetTokens = [];
    private readonly List<RefreshToken> _refreshTokens = [];
    private readonly List<LoginLog> _loginLogs = [];

    public string FullName { get; set; } = string.Empty;
    public AccountStatus Status { get; set; } = AccountStatus.PendingVerification;
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockedUntil { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "system";
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public IReadOnlyCollection<OtpCode> OtpCodes => _otpCodes;
    public IReadOnlyCollection<PasswordResetToken> PasswordResetTokens => _passwordResetTokens;
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;
    public IReadOnlyCollection<LoginLog> LoginLogs => _loginLogs;
   
    public byte[] RowVersion { get; set; } = [];
}
