using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Attempts.Timer;

public record GetAttemptTimerQuery(
    Guid AttemptId,
    Guid StudentId
) : IQuery<RequestResult<GetAttemptTimerResponse>>;

public record GetAttemptTimerResponse(int SecondsRemaining);

public class GetAttemptTimerQueryHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider
) : IRequestHandler<GetAttemptTimerQuery, RequestResult<GetAttemptTimerResponse>>
{
    public async Task<RequestResult<GetAttemptTimerResponse>> Handle(GetAttemptTimerQuery request, CancellationToken cancellationToken)
    {
        var attemptRepository = unitOfWork.Repository<QuizAttempt>();
        var attempt = await attemptRepository.GetByIdAsync(request.AttemptId);

        if (attempt is null)
            return RequestResult<GetAttemptTimerResponse>.Failure(null!, ResultCode.AttemptNotFound);

        if (attempt.UserId != request.StudentId)
            return RequestResult<GetAttemptTimerResponse>.Failure(null!, ResultCode.AttemptNotOwned);

        if (attempt.Status == QuizAttemptStatus.Submitting)
            return RequestResult<GetAttemptTimerResponse>.Failure(null!, ResultCode.AttemptTimedOut);

        var now = dateTimeProvider.UtcNow;
        if (attempt.Status == QuizAttemptStatus.InProgress && now > attempt.Deadline)
        {
            return RequestResult<GetAttemptTimerResponse>.Failure(null!, ResultCode.AttemptTimedOut);
        }

        if (attempt.Status == QuizAttemptStatus.Expired)
            return RequestResult<GetAttemptTimerResponse>.Failure(null!, ResultCode.AttemptTimedOut);

        if (attempt.Status != QuizAttemptStatus.InProgress)
            return RequestResult<GetAttemptTimerResponse>.Failure(null!, ResultCode.AttemptAlreadySubmitted);

        var secondsRemaining = (int)Math.Max(0, (attempt.Deadline - now).TotalSeconds);
        return RequestResult<GetAttemptTimerResponse>.succeeded(
            new GetAttemptTimerResponse(secondsRemaining),
            ResultCode.AttemptDetailsRetrievedSuccessfully);
    }
}
