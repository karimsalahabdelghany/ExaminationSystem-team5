using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Enums;
using System.Data;
using System.Text.Json;

namespace ExaminationSystem.Application.Features.Attempts.SubmitAttempt;

public record SubmitAttemptOrchestrator(
    Guid AttemptId,
    Guid StudentId
) : ICommand<SubmitAttemptResult>;

public record SubmitAttemptResult(
    bool AlreadySubmitted,
    SubmitAttemptResponse Value
);

public record SubmitAttemptResponse(
    Guid AttemptId,
    decimal Score,
    bool Passed
);

public class SubmitAttemptOrchestratorHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<SubmitAttemptOrchestratorHandler> logger
) : IRequestHandler<SubmitAttemptOrchestrator, SubmitAttemptResult>
{
    public async Task<SubmitAttemptResult> Handle(SubmitAttemptOrchestrator request, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var attemptRepository = unitOfWork.Repository<QuizAttempt>();

            var attempt = await attemptRepository.GetByIdAsync(request.AttemptId);
            if (attempt is null)
                throw new NotFoundException("Attempt", request.AttemptId);

            if (attempt.UserId != request.StudentId)
                throw new ForbiddenException("You do not own this attempt.");

            if (attempt.Status != QuizAttemptStatus.InProgress)
            {
                var existingResult = await GetExistingResult(request.AttemptId);
                if (existingResult is not null)
                {
                    logger.LogInformation(
                        "Submit skipped for attempt {AttemptId}; already submitted with existing result.",
                        request.AttemptId);
                    await unitOfWork.CommitAsync(cancellationToken);
                    return new SubmitAttemptResult(true, existingResult);
                }

                throw new ConflictException("Attempt", "Attempt is already submitted.");
            }

            var now = dateTimeProvider.UtcNow;
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

            return new SubmitAttemptResult(
                AlreadySubmitted: false,
                Value: new SubmitAttemptResponse(
                    AttemptId: attempt.Id,
                    Score: scorePayload.Score,
                    Passed: scorePayload.Passed
                ));
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
                return new SubmitAttemptResult(true, existingResult);
            }

            throw new ConflictException("Attempt", "Attempt is already submitted.");
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
        var existing = await resultRepository
            .GetAll(r => r.AttemptId == attemptId)
            .Select(r => new SubmitAttemptResponse(
                r.AttemptId,
                r.Score,
                r.Passed
            ))
            .FirstOrDefaultAsync();

        return existing;
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
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Quiz", quizId);

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
