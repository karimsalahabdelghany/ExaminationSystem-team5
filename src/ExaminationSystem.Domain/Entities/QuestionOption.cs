namespace ExaminationSystem.Domain.Entities;

public class QuestionOption : BaseEntity
{
    private readonly List<AttemptAnswer> _attemptAnswers = [];

    private QuestionOption()
    {
    }

    public QuestionOption(Guid questionId, string text, bool isCorrect, int orderIndex)
    {
        QuestionId = questionId;
        Text = text;
        IsCorrect = isCorrect;
        OrderIndex = orderIndex;
    }

    public Guid QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }

    public Question Question { get; set; } = null!;
    public IReadOnlyCollection<AttemptAnswer> AttemptAnswers => _attemptAnswers;
}
