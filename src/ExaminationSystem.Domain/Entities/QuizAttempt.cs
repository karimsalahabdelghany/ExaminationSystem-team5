namespace ExaminationSystem.Domain.Entities;

public class QuizAttempt : BaseEntity
{
    private readonly List<AttemptAnswer> _answers = [];

    private QuizAttempt()
    {
    }

    public QuizAttempt(Guid userId, Guid quizId, QuizAttemptStatus status, DateTime startTime, DateTime deadline)
    {
        UserId = userId;
        QuizId = quizId;
        Status = status;
        StartTime = startTime;
        Deadline = deadline;
    }

    public Guid UserId { get; set; }
    public Guid QuizId { get; set; }
    public QuizAttemptStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public Student Student { get; set; } = null!;
    public Quiz Quiz { get; set; } = null!;
    public IReadOnlyCollection<AttemptAnswer> Answers => _answers;
    public AttemptResult? Result { get; set; }

    public void AttachResult(AttemptResult result)
    {
        Result = result;
    }
}
