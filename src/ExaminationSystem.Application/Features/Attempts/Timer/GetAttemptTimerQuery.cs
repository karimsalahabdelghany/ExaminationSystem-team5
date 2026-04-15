using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Application.Features.Attempts.SubmitAttempt;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Attempts.Timer;

public record GetAttemptTimerQuery(
    Guid AttemptId,
    Guid StudentId
) : IQuery<GetAttemptTimerResponse>;

public record GetAttemptTimerResponse(int SecondsRemaining);

public class GetAttemptTimerQueryHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    IMediator mediator
) : IRequestHandler<GetAttemptTimerQuery, GetAttemptTimerResponse>
{
    public async Task<GetAttemptTimerResponse> Handle(GetAttemptTimerQuery request, CancellationToken cancellationToken)
    {
        var attemptRepository = unitOfWork.Repository<QuizAttempt>();
        var attempt = await attemptRepository.GetByIdAsync(request.AttemptId);

        if (attempt is null)
            throw new NotFoundException("Attempt", request.AttemptId);

        if (attempt.UserId != request.StudentId)
            throw new ForbiddenException("You do not own this attempt.");

        var now = dateTimeProvider.UtcNow;
        if (attempt.Status == QuizAttemptStatus.InProgress && now > attempt.Deadline)
        {
            await mediator.Send(new SubmitAttemptOrchestrator(request.AttemptId, request.StudentId), cancellationToken);
            throw new GoneException("Time has expired. Your attempt has been auto-submitted.");
        }

        if (attempt.Status == QuizAttemptStatus.Expired)
            throw new GoneException("Time has expired. Your attempt has been auto-submitted.");

        if (attempt.Status != QuizAttemptStatus.InProgress)
            throw new ConflictException("Attempt", "Attempt is already submitted.");

        var secondsRemaining = (int)Math.Max(0, (attempt.Deadline - now).TotalSeconds);
        return new GetAttemptTimerResponse(secondsRemaining);
    }
}
