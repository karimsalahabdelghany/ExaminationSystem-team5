using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Enums;
using System.Data;
using System.Text.Json;

namespace ExaminationSystem.Application.Features.Attempts.SubmitAttempt;

public record SubmitAttemptOrchestrator(
    Guid AttemptId,
    Guid StudentId
) : ICommand<RequestResult<SubmitAttemptResponse>>;

public record SubmitAttemptResponse(
    Guid AttemptId,
    decimal Score,
    bool Passed
);

public class SubmitAttemptOrchestratorHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    IQuizAttemptAutoSubmitClaim submitClaim,
    ILogger<SubmitAttemptOrchestratorHandler> logger
) : IRequestHandler<SubmitAttemptOrchestrator, RequestResult<SubmitAttemptResponse>>
{
    public async Task<RequestResult<SubmitAttemptResponse>> Handle(
        SubmitAttemptOrchestrator request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var attemptRepository = unitOfWork.Repository<QuizAttempt>();
            var attempt = await attemptRepository.GetByIdAsync(request.AttemptId);
            if (attempt is null)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<SubmitAttemptResponse>.Failure(null!, ResultCode.AttemptNotFound);
            }

            if (attempt.UserId != request.StudentId)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<SubmitAttemptResponse>.Failure(null!, ResultCode.AttemptNotOwned);
            }

            var now = dateTimeProvider.UtcNow;

            if (attempt.Status is QuizAttemptStatus.Submitted or QuizAttemptStatus.Expired)
            {
                var existing = await GetExistingResult(request.AttemptId);
                if (existing is not null)
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                    logger.LogInformation(
                        "Submit idempotent success for attempt {AttemptId} (already finalized).",
                        request.AttemptId);
                    return RequestResult<SubmitAttemptResponse>.succeeded(
                        existing,
                        ResultCode.SubmitAttemptSuccessful);
                }

                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<SubmitAttemptResponse>.Failure(null!, ResultCode.AttemptAlreadySubmitted);
            }

            if (attempt.Status == QuizAttemptStatus.Submitting)
            {
                var existingWhileSubmitting = await GetExistingResult(request.AttemptId);
                if (existingWhileSubmitting is not null)
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                    return RequestResult<SubmitAttemptResponse>.succeeded(
                        existingWhileSubmitting,
                        ResultCode.SubmitAttemptSuccessful);
                }
            }

            if (attempt.Status == QuizAttemptStatus.InProgress && now > attempt.Deadline)
            {
                var claimed = await submitClaim.TryClaimInProgressOverdueAsync(
                    request.AttemptId,
                    now,
                    cancellationToken);
                if (claimed > 0)
                {
                    logger.LogDebug(
                        "Claimed overdue attempt {AttemptId} for auto-submit (rows={Rows}).",
                        request.AttemptId,
                        claimed);
                }

                attempt = await attemptRepository.GetByIdAsync(request.AttemptId);
                if (attempt is null)
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                    return RequestResult<SubmitAttemptResponse>.Failure(null!, ResultCode.AttemptNotFound);
                }
            }

            if (attempt.Status is not (QuizAttemptStatus.InProgress or QuizAttemptStatus.Submitting))
            {
                var lateExisting = await GetExistingResult(request.AttemptId);
                if (lateExisting is not null)
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                    return RequestResult<SubmitAttemptResponse>.succeeded(
                        lateExisting,
                        ResultCode.SubmitAttemptSuccessful);
                }

                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<SubmitAttemptResponse>.Failure(null!, ResultCode.AttemptAlreadySubmitted);
            }

            var isTimedOut = now > attempt.Deadline;
            attempt.Status = isTimedOut ? QuizAttemptStatus.Expired : QuizAttemptStatus.Submitted;
            attempt.SubmittedAt = now;

            var scorePayload = await BuildScorePayload(attempt.QuizId, attempt.Id, attempt.Deadline, cancellationToken);

            attemptRepository.Update(attempt);

            var resultRepository = unitOfWork.Repository<AttemptResult>();
            resultRepository.Add(new AttemptResult(
                attemptId: attempt.Id,
                score: scorePayload.Score,
                passed: scorePayload.Passed,
                totalQuestions: scorePayload.TotalQuestions,
                correctCount: scorePayload.CorrectCount,
                calculatedAt: now,
                questionBreakdownJson: scorePayload.QuestionBreakdownJson
            ));

            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Attempt {AttemptId} submitted with score {Score}. TimedOut={IsTimedOut}.",
                attempt.Id,
                scorePayload.Score,
                isTimedOut);

            var response = new SubmitAttemptResponse(attempt.Id, scorePayload.Score, scorePayload.Passed);
            return isTimedOut
                ? RequestResult<SubmitAttemptResponse>.Failure(response, ResultCode.AttemptTimedOut)
                : RequestResult<SubmitAttemptResponse>.succeeded(response, ResultCode.SubmitAttemptSuccessful);
        }
        catch (DbUpdateException ex) when (IsDuplicateAttemptResultViolation(ex))
        {
            await unitOfWork.RollbackAsync(cancellationToken);

            var existingResult = await GetExistingResult(request.AttemptId);
            if (existingResult is not null)
            {
                logger.LogInformation(
                    "Concurrent submit handled by returning existing result for attempt {AttemptId}.",
                    request.AttemptId);
                var attemptRepository = unitOfWork.Repository<QuizAttempt>();
                return RequestResult<SubmitAttemptResponse>.succeeded(
                    existingResult,
                    ResultCode.SubmitAttemptSuccessful);
            }

            return RequestResult<SubmitAttemptResponse>.Failure(null!, ResultCode.AttemptAlreadySubmitted);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<SubmitAttemptResponse?> GetExistingResult(Guid attemptId)
    {
        var resultRepository = unitOfWork.Repository<AttemptResult>();
        return await resultRepository
            .GetAll(r => r.AttemptId == attemptId)
            .Select(r => new SubmitAttemptResponse(
                r.AttemptId,
                r.Score,
                r.Passed
            ))
            .FirstOrDefaultAsync();
    }

    private async Task<ScorePayload> BuildScorePayload(
        Guid quizId,
        Guid attemptId,
        DateTime deadline,
        CancellationToken cancellationToken)
    {
        var quizRepository = unitOfWork.Repository<Quiz>();
        var answerRepository = unitOfWork.Repository<AttemptAnswer>();

        var quizSnapshot = await quizRepository
            .GetAll(q => q.Id == quizId)
            .Select(q => new
            {
                q.PassScore,
                QuestionIds = q.Questions.Select(question => question.Id).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (quizSnapshot is null)
            throw new InvalidOperationException($"Quiz {quizId} not found while scoring attempt.");

        var answers = await answerRepository
            .GetAll(a => a.AttemptId == attemptId)
            .Select(a => new
            {
                a.QuestionId,
                a.SelectedOptionId,
                a.AnsweredAt,
                IsCorrect = a.SelectedOption.IsCorrect
            })
            .ToListAsync(cancellationToken);

        var totalQuestions = quizSnapshot.QuestionIds.Count;
        var answersBeforeDeadline = answers
            .Where(answer => answer.AnsweredAt <= deadline)
            .ToDictionary(answer => answer.QuestionId, answer => answer);

        var correctCount = answersBeforeDeadline.Count(answer => answer.Value.IsCorrect);
        var score = totalQuestions == 0
            ? 0m
            : Math.Round((decimal)correctCount / totalQuestions * 100m, 2, MidpointRounding.AwayFromZero);
        var passed = score >= quizSnapshot.PassScore;

        var breakdown = quizSnapshot.QuestionIds
            .Select(questionId =>
            {
                var hasAnswer = answersBeforeDeadline.TryGetValue(questionId, out var answer);
                return new QuestionBreakdownItem(
                    QuestionId: questionId,
                    SelectedOptionId: hasAnswer ? answer!.SelectedOptionId : null,
                    IsCorrect: hasAnswer && answer!.IsCorrect
                );
            })
            .ToList();

        return new ScorePayload(
            TotalQuestions: totalQuestions,
            CorrectCount: correctCount,
            Score: score,
            Passed: passed,
            QuestionBreakdownJson: JsonSerializer.Serialize(breakdown)
        );
    }

    private static bool IsDuplicateAttemptResultViolation(DbUpdateException exception)
    {
        const string uniqueAttemptResultIndex = "IX_AttemptResults_AttemptId";
        var message = exception.InnerException?.Message ?? exception.Message;
        return message.Contains(uniqueAttemptResultIndex, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record ScorePayload(
        int TotalQuestions,
        int CorrectCount,
        decimal Score,
        bool Passed,
        string QuestionBreakdownJson
    );

    private sealed record QuestionBreakdownItem(
        Guid QuestionId,
        Guid? SelectedOptionId,
        bool IsCorrect
    );
}
