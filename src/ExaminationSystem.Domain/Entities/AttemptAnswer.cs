namespace ExaminationSystem.Domain.Entities;

public class AttemptAnswer : BaseEntity
{
    private readonly List<AttemptAnswerOption> _selectedOptions = [];

    private AttemptAnswer()
    {
    }

    public AttemptAnswer(Guid attemptId, Guid questionId)
    {
        AttemptId = attemptId;
        QuestionId = questionId;
    }

    public Guid AttemptId { get; private set; }
    public Guid QuestionId { get; private set; }

    public QuizAttempt Attempt { get; private set; } = null!;
    public Question Question { get; private set; } = null!;
    public IReadOnlyCollection<AttemptAnswerOption> SelectedOptions => _selectedOptions;
}

