using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Attempts.Commands;

public record MarkAttemptTimedOutCommand(Guid AttemptId) : ICommand<Unit>;


public class MarkAttemptTimedOutCommandHandler(
    IRepository<QuizAttempt> attemptRepository) : IRequestHandler<MarkAttemptTimedOutCommand, Unit>
{
    public async Task<Unit> Handle(MarkAttemptTimedOutCommand request, CancellationToken cancellationToken)
    {
        var attempt = await attemptRepository.GetByIdAsync(request.AttemptId);

        if (attempt is null)
            throw new NotFoundException("Attempt", request.AttemptId);

        attempt.Status = QuizAttemptStatus.Expired;
        attempt.SubmittedAt = DateTime.UtcNow;
        attemptRepository.Update(attempt);

        return Unit.Value;
    }
}
