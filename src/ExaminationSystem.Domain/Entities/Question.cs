using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Domain.Entities;

public class Question : BaseEntity
{
    private readonly List<QuestionOption> _options = [];
    private readonly List<AttemptAnswer> _attemptAnswers = [];

    private Question()
    {
    }

    public Question(Guid quizId, string text, QuestionType type)
    {
        QuizId = quizId;
        Text = text;
        Type = type;
    }

    public Guid QuizId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public QuestionType Type { get; private set; }

    public Quiz Quiz { get; private set; } = null!;
    public IReadOnlyCollection<QuestionOption> Options => _options;
    public IReadOnlyCollection<AttemptAnswer> AttemptAnswers => _attemptAnswers;
}

