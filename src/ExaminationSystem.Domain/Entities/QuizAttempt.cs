using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Domain.Entities;

public class QuizAttempt : BaseEntity
{
    private readonly List<AttemptAnswer> _answers = [];

    private QuizAttempt()
    {
    }

    public QuizAttempt(Guid userId, Guid quizId, DateTime startTime, QuizAttemptStatus status)
    {
        UserId = userId;
        QuizId = quizId;
        StartTime = startTime;
        Status = status;
    }

    public Guid UserId { get; private set; }
    public Guid QuizId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public QuizAttemptStatus Status { get; private set; }

    public User User { get; private set; } = null!;
    public Quiz Quiz { get; private set; } = null!;
    public IReadOnlyCollection<AttemptAnswer> Answers => _answers;
    public AttemptResult Result { get; private set; } = null!;
}

