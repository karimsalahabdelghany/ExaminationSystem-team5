namespace ExaminationSystem.Domain.Enums;

public enum QuizAttemptStatus : byte
{
    NotStarted = 0,
    InProgress = 1,
    Submitted = 2,
    Expired = 3,
    Graded = 4,
}
