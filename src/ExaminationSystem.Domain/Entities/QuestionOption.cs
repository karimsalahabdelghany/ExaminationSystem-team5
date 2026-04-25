namespace ExaminationSystem.Domain.Entities;

public class QuestionOption : BaseEntity
{
    private readonly List<AttemptAnswer> _attemptAnswers = [];

    public Guid QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }

    public Question Question { get; set; } = null!;
    public IReadOnlyCollection<AttemptAnswer> AttemptAnswers => _attemptAnswers;
}
