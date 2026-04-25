namespace ExaminationSystem.Domain.Enums;

public enum OtpPurpose : byte
{
    EmailConfirmation = 0,
    PasswordReset = 1,
    Login = 2,
    AccountVerification = 3,
    EmailChange = 4,
}
