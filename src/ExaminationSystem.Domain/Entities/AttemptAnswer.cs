namespace ExaminationSystem.Domain.Entities;

public class AttemptAnswer : BaseEntity
{
    private AttemptAnswer()
    {
    }

    public AttemptAnswer(Guid attemptId, Guid questionId, Guid selectedOptionId, DateTime answeredAt)
    {
        AttemptId = attemptId;
        QuestionId = questionId;
        SelectedOptionId = selectedOptionId;
        AnsweredAt = answeredAt;
    }

    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
    public DateTime AnsweredAt { get; set; }

    public QuizAttempt Attempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public QuestionOption SelectedOption { get; set; } = null!;
}
