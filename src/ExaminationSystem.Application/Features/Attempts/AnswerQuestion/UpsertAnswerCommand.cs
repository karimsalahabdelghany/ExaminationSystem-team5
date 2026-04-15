using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;

namespace ExaminationSystem.Application.Features.Attempts.AnswerQuestion;

public record UpsertAnswerCommand(
    Guid AttemptId,
    Guid QuestionId,
    Guid SelectedOptionId
) : ICommand<Unit>;


public class UpsertAnswerCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<UpsertAnswerCommand, Unit>
{
    public async Task<Unit> Handle(UpsertAnswerCommand request, CancellationToken cancellationToken)
    {
        var answerRepository = unitOfWork.Repository<AttemptAnswer>();

        var existing = await answerRepository
            .FindAsync(a => a.AttemptId == request.AttemptId && a.QuestionId == request.QuestionId);

        if (existing is not null)
        {
            existing.SelectedOptionId = request.SelectedOptionId;
            existing.AnsweredAt = DateTime.UtcNow;
            answerRepository.Update(existing);
        }
        else
        {
            answerRepository.Add(new AttemptAnswer(
                attemptId: request.AttemptId,
                questionId: request.QuestionId,
                selectedOptionId: request.SelectedOptionId,
                answeredAt: DateTime.UtcNow
            ));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
