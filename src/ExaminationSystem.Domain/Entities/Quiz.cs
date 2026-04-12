namespace ExaminationSystem.Domain.Entities;

public class Quiz : BaseEntity
{
    private readonly List<Question> _questions = [];
    private readonly List<QuizAttempt> _quizAttempts = [];

    private Quiz()
    {
    }

    public Quiz(Guid diplomaId, string title, string instructions, int durationMinutes, int passScore, int maxAttempts, QuizStatus status)
    {
        DiplomaId = diplomaId;
        Title = title;
        Instructions = instructions;
        DurationMinutes = durationMinutes;
        PassScore = passScore;
        MaxAttempts = maxAttempts;
        Status = status;
    }

    public Guid DiplomaId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int PassScore { get; set; }
    public int MaxAttempts { get; set; }
    public QuizStatus Status { get; set; }

    public Diploma Diploma { get; set; } = null!;
    public IReadOnlyCollection<Question> Questions => _questions;
    public IReadOnlyCollection<QuizAttempt> QuizAttempts => _quizAttempts;
}
