using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;
using System.Buffers.Binary;
using System.Data;
using System.Security.Cryptography;

namespace ExaminationSystem.Application.Features.Quizzes.StartQuizAttempt;

public record StartQuizAttemptCommand(Guid QuizId, Guid StudentId)
    : ICommand<RequestResult<StartQuizAttemptResponse>>;

public class StartQuizAttemptCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<StartQuizAttemptCommandHandler> logger)
    : IRequestHandler<StartQuizAttemptCommand, RequestResult<StartQuizAttemptResponse>>
{
    public async Task<RequestResult<StartQuizAttemptResponse>> Handle(
        StartQuizAttemptCommand request, CancellationToken cancellationToken)
    {
        var quiz = await GetQuiz(request.QuizId, cancellationToken);
        if (quiz is null)
            return RequestResult<StartQuizAttemptResponse>.Failure(null!, ResultCode.QuizNotFoundOrNotPublished);

        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var existing = await GetExistingAttempt(request.QuizId, request.StudentId);
            if (existing is not null)
            {
                var response = ToResponse(existing, quiz, request, concurrentRecovery: false);
                await unitOfWork.CommitAsync(cancellationToken);
                return RequestResult<StartQuizAttemptResponse>.Failure(response, ResultCode.AttemptAlreadyInProgress);
            }

            var attempts = unitOfWork.Repository<QuizAttempt>();
            var count = await attempts.CountAsync(a =>
                a.QuizId == request.QuizId && a.UserId == request.StudentId);
            if (count >= quiz.MaxAttempts)
            {
                logger.LogWarning(
                    "Attempt limit reached for student {StudentId} and quiz {QuizId}. Attempts: {AttemptCount}, Max: {MaxAttempts}.",
                    request.StudentId, request.QuizId, count, quiz.MaxAttempts);
                await unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<StartQuizAttemptResponse>.Failure(null!, ResultCode.AttemptLimitReached);
            }

            var created = CreateNewAttempt(request, quiz);
            await unitOfWork.CommitAsync(cancellationToken);
            return RequestResult<StartQuizAttemptResponse>.succeeded(created, ResultCode.QuizAttemptStartedSuccessfully);
        }
        catch (DbUpdateException ex) when (IsInProgressAttemptConstraintViolation(ex))
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            var recovered = await GetExistingAttempt(request.QuizId, request.StudentId);
            if (recovered is null)
                return RequestResult<StartQuizAttemptResponse>.Failure(null!, ResultCode.AttemptStartConflict);
            return RequestResult<StartQuizAttemptResponse>.Failure(
                ToResponse(recovered, quiz, request, concurrentRecovery: true),
                ResultCode.AttemptAlreadyInProgress);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<QuizStartProjection?> GetQuiz(Guid quizId, CancellationToken cancellationToken) =>
        await unitOfWork.Repository<Quiz>()
            .GetAll(q => q.Id == quizId && q.Status == QuizStatus.Published)
            .Select(q => new QuizStartProjection(
                q.Id, q.Title, q.Instructions, q.DurationMinutes, q.PassScore, q.MaxAttempts,
                q.Questions.Select(question => new QuestionStartProjection(
                    question.Id, question.Text, question.Type,
                    question.Options.Select(o => new QuestionOptionStartProjection(o.Id, o.Text)).ToList())).ToList()))
            .FirstOrDefaultAsync(cancellationToken);

    private Task<QuizAttempt?> GetExistingAttempt(Guid quizId, Guid studentId) =>
        unitOfWork.Repository<QuizAttempt>().FindAsync(a =>
            a.QuizId == quizId
            && a.UserId == studentId
            && (a.Status == QuizAttemptStatus.InProgress || a.Status == QuizAttemptStatus.Submitting));

    private StartQuizAttemptResponse ToResponse(
        QuizAttempt attempt, QuizStartProjection quiz, StartQuizAttemptCommand request, bool concurrentRecovery)
    {
        logger.LogInformation(
            "Returning existing in-progress attempt {AttemptId} for student {StudentId} and quiz {QuizId}. ConcurrentRecovery={ConcurrentRecovery}",
            attempt.Id, request.StudentId, request.QuizId, concurrentRecovery);
        return new(attempt.Id, attempt.StartTime, attempt.Deadline, Meta(quiz), Shuffle(attempt.Id, quiz.Questions));
    }

    private StartQuizAttemptResponse CreateNewAttempt(StartQuizAttemptCommand request, QuizStartProjection quiz)
    {
        var start = dateTimeProvider.UtcNow;
        var deadline = start.AddMinutes(quiz.DurationMinutes);
        var attempt = unitOfWork.Repository<QuizAttempt>().Add(new QuizAttempt(
            request.StudentId, request.QuizId, QuizAttemptStatus.InProgress, start, deadline));
        logger.LogInformation(
            "Started new attempt {AttemptId} for student {StudentId} and quiz {QuizId}.",
            attempt.Id, request.StudentId, request.QuizId);
        return new(attempt.Id, start, deadline, Meta(quiz), Shuffle(attempt.Id, quiz.Questions));
    }

    private static StartQuizMetadataResponse Meta(QuizStartProjection q) =>
        new(q.Id, q.Title, q.Instructions, q.DurationMinutes, q.PassScore, q.MaxAttempts);

    private static IReadOnlyCollection<StartQuizQuestionResponse> Shuffle(
        Guid attemptId, IReadOnlyCollection<QuestionStartProjection> questions) =>
        [.. questions
            .OrderBy(x => StableOrder(attemptId, x.Id))
            .Select(q => new StartQuizQuestionResponse(q.Id, q.Text, q.Type,
                [.. q.Options.OrderBy(o => StableOrder(attemptId, o.Id))
                    .Select(o => new StartQuizQuestionOptionResponse(o.Id, o.Text))]))];

    private static ulong StableOrder(Guid attemptId, Guid itemId)
    {
        Span<byte> buf = stackalloc byte[32], hash = stackalloc byte[32];
        attemptId.TryWriteBytes(buf[..16]);
        itemId.TryWriteBytes(buf[16..]);
        SHA256.TryHashData(buf, hash, out _);
        return BinaryPrimitives.ReadUInt64LittleEndian(hash[..8]);
    }

    private static bool IsInProgressAttemptConstraintViolation(DbUpdateException ex)
    {
        var msg = ex.InnerException?.Message ?? ex.Message;
        return msg.Contains("UX_QuizAttempts_UserId_QuizId_InProgress", StringComparison.OrdinalIgnoreCase);
    }
}
