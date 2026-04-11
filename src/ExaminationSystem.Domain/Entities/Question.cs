namespace ExaminationSystem.Domain.Entities;

public class Question : BaseEntity
{
    private readonly List<QuestionOption> _options = [];
    private readonly List<AttemptAnswer> _attemptAnswers = [];

    private Question()
    {
    }

    public Question(Guid quizId, string text, QuestionType type, string? explanation, int orderIndex)
    {
        QuizId = quizId;
        Text = text;
        Type = type;
        Explanation = explanation;
        OrderIndex = orderIndex;
    }

    public Guid QuizId { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public string? Explanation { get; set; }
    public int OrderIndex { get; set; }

    public Quiz Quiz { get; set; } = null!;
    public IReadOnlyCollection<QuestionOption> Options => _options;
    public IReadOnlyCollection<AttemptAnswer> AttemptAnswers => _attemptAnswers;
}
