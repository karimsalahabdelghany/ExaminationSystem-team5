namespace ExaminationSystem.Domain.Enums;

public enum AccountStatus : byte
{
    Active = 0,
    Locked = 1,
    Suspended = 2,
    PendingVerification = 3,
}
