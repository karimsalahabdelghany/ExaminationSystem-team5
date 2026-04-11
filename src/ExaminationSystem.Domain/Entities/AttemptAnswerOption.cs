namespace ExaminationSystem.Domain.Entities;

public class AttemptAnswerOption : BaseEntity
{
    private AttemptAnswerOption()
    {
    }

    public AttemptAnswerOption(Guid attemptAnswerId, Guid selectedOptionId)
    {
        AttemptAnswerId = attemptAnswerId;
        SelectedOptionId = selectedOptionId;
    }

    public Guid AttemptAnswerId { get; private set; }
    public Guid SelectedOptionId { get; private set; }

    public AttemptAnswer AttemptAnswer { get; private set; } = null!;
    public QuestionOption SelectedOption { get; private set; } = null!;
}

