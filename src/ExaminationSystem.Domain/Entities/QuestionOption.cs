namespace ExaminationSystem.Domain.Entities;

public class QuestionOption : BaseEntity
{
    private readonly List<AttemptAnswerOption> _attemptAnswerOptions = [];

    private QuestionOption()
    {
    }

    public QuestionOption(Guid questionId, string text, bool isCorrect)
    {
        QuestionId = questionId;
        Text = text;
        IsCorrect = isCorrect;
    }

    public Guid QuestionId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public bool IsCorrect { get; private set; }

    public Question Question { get; private set; } = null!;
    public IReadOnlyCollection<AttemptAnswerOption> AttemptAnswerOptions => _attemptAnswerOptions;
}

