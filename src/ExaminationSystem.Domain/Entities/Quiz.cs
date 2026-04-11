namespace ExaminationSystem.Domain.Entities;

public class Quiz : BaseEntity
{
    private readonly List<Question> _questions = [];
    private readonly List<QuizAttempt> _quizAttempts = [];

    private Quiz()
    {
    }

    public Quiz(Guid diplomaId, string title, int duration, int maxAttempts)
    {
        DiplomaId = diplomaId;
        Title = title;
        Duration = duration;
        MaxAttempts = maxAttempts;
    }

    public Guid DiplomaId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int Duration { get; private set; }
    public int MaxAttempts { get; private set; }

    public Diploma Diploma { get; private set; } = null!;
    public IReadOnlyCollection<Question> Questions => _questions;
    public IReadOnlyCollection<QuizAttempt> QuizAttempts => _quizAttempts;
}

