namespace ExaminationSystem.Domain.Entities;

public class Question : BaseEntity
{
    //private readonly List<QuestionOption> _options = [];
    //private readonly List<AttemptAnswer> _attemptAnswers = [];

    public Guid QuizId { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public string? Explanation { get; set; }
    public int OrderIndex { get; set; }

    public Quiz Quiz { get; set; } = null!;
    public List<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    public List<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
}
