namespace ExaminationSystem.Domain.Enums;

public enum OtpPurpose : byte
{
    EmailVerification = 1,
    TwoFactorAuthentication = 2,
    PasswordReset = 3
}
