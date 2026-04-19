using ExaminationSystem.Domain.Enums;
using System.Buffers.Binary;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ExaminationSystem.Application.Features.Quizzes.StartQuizAttempt;

public record StartQuizAttemptCommand(
    Guid QuizId,
    Guid StudentId
) : ICommand<StartQuizAttemptResult>;

public record StartQuizAttemptResult(
    bool HasInProgressAttempt,
    StartQuizAttemptResponse? Value
);

public record StartQuizAttemptResponse(
    Guid AttemptId,
    DateTime StartTime,
    DateTime Deadline,
    StartQuizMetadataResponse Quiz,
    IReadOnlyCollection<StartQuizQuestionResponse> Questions
);

public record StartQuizMetadataResponse(
    Guid Id,
    string Title,
    string Instructions,
    int DurationMinutes,
    int PassScore,
    int MaxAttempts
);

public record StartQuizQuestionResponse(
    Guid Id,
    string Text,
    QuestionType Type,
    IReadOnlyCollection<StartQuizQuestionOptionResponse> Options
);

public record StartQuizQuestionOptionResponse(
    Guid Id,
    string Text
);

public class StartQuizAttemptCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<StartQuizAttemptCommandHandler> logger
) : IRequestHandler<StartQuizAttemptCommand, StartQuizAttemptResult>
{
    public async Task<StartQuizAttemptResult> Handle(StartQuizAttemptCommand request, CancellationToken cancellationToken)
    {
        var quiz = await GetQuiz(request.QuizId, cancellationToken);

        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var existingAttempt = await GetExistingAttempt(request.QuizId, request.StudentId);

            if (existingAttempt is not null)
            {
                var existingAttemptResult = HandleExistingAttempt(request, quiz, existingAttempt);
                await unitOfWork.CommitAsync(cancellationToken);
                return existingAttemptResult;
            }

            var newAttemptResult = await CreateNewAttempt(request, quiz);
            await unitOfWork.CommitAsync(cancellationToken);
            return newAttemptResult;
        }
        catch (DbUpdateException ex) when (IsInProgressAttemptConstraintViolation(ex))
        {
            await unitOfWork.RollbackAsync(cancellationToken);

            var existingAttempt = await GetExistingAttempt(request.QuizId, request.StudentId);

            if (existingAttempt is not null)
            {
                return HandleExistingAttempt(request, quiz, existingAttempt, isConcurrentRecovery: true);
            }

            throw new ConflictException("Attempt already in progress");
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<QuizStartProjection> GetQuiz(Guid quizId, CancellationToken cancellationToken)
    {
        var quizRepository = unitOfWork.Repository<Quiz>();

        var quiz = await quizRepository
            .GetAll(q => q.Id == quizId && q.Status == QuizStatus.Published)
            .Select(q => new QuizStartProjection(
                q.Id,
                q.Title,
                q.Instructions,
                q.DurationMinutes,
                q.PassScore,
                q.MaxAttempts,
                q.Questions.Select(question => new QuestionStartProjection(
                    question.Id,
                    question.Text,
                    question.Type,
                    question.Options.Select(option => new QuestionOptionStartProjection(
                        option.Id,
                        option.Text
                    )).ToList()
                )).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (quiz is null)
            throw new NotFoundException("Quiz not found or not published.");

        return quiz;
    }

    private async Task<QuizAttempt?> GetExistingAttempt(Guid quizId, Guid studentId)
    {
        var attemptRepository = unitOfWork.Repository<QuizAttempt>();
        return await attemptRepository.FindAsync(a =>
            a.QuizId == quizId &&
            a.UserId == studentId &&
            a.Status == QuizAttemptStatus.InProgress);
    }

    private StartQuizAttemptResult HandleExistingAttempt(
        StartQuizAttemptCommand request,
        QuizStartProjection quiz,
        QuizAttempt existingAttempt,
        bool isConcurrentRecovery = false)
    {
        var existingAttemptQuestions = BuildShuffledQuestions(existingAttempt.Id, quiz.Questions);

        logger.LogInformation(
            "Returning existing in-progress attempt {AttemptId} for student {StudentId} and quiz {QuizId}. ConcurrentRecovery={IsConcurrentRecovery}",
            existingAttempt.Id,
            request.StudentId,
            request.QuizId,
            isConcurrentRecovery);

        return new StartQuizAttemptResult(
            HasInProgressAttempt: true,
            Value: new StartQuizAttemptResponse(
                AttemptId: existingAttempt.Id,
                StartTime: existingAttempt.StartTime,
                Deadline: existingAttempt.Deadline,
                Quiz: BuildQuizMetadata(quiz),
                Questions: existingAttemptQuestions
            )
        );
    }

    private async Task<StartQuizAttemptResult> CreateNewAttempt(
        StartQuizAttemptCommand request,
        QuizStartProjection quiz)
    {
        var attemptRepository = unitOfWork.Repository<QuizAttempt>();

        var attemptCount = await attemptRepository.CountAsync(a =>
            a.QuizId == request.QuizId &&
            a.UserId == request.StudentId);

        if (attemptCount >= quiz.MaxAttempts)
        {
            logger.LogWarning(
                "Attempt limit reached for student {StudentId} and quiz {QuizId}. Attempts: {AttemptCount}, Max: {MaxAttempts}.",
                request.StudentId,
                request.QuizId,
                attemptCount,
                quiz.MaxAttempts);
            throw new ForbiddenException("Attempt limit reached");
        }

        var startTime = dateTimeProvider.UtcNow;
        var deadline = startTime.AddMinutes(quiz.DurationMinutes);

        var attempt = attemptRepository.Add(new QuizAttempt(
            userId: request.StudentId,
            quizId: request.QuizId,
            status: QuizAttemptStatus.InProgress,
            startTime: startTime,
            deadline: deadline
        ));

        var shuffledQuestions = BuildShuffledQuestions(attempt.Id, quiz.Questions);

        logger.LogInformation(
            "Started new attempt {AttemptId} for student {StudentId} and quiz {QuizId}.",
            attempt.Id,
            request.StudentId,
            request.QuizId);

        return new StartQuizAttemptResult(
            HasInProgressAttempt: false,
            Value: new StartQuizAttemptResponse(
                AttemptId: attempt.Id,
                StartTime: startTime,
                Deadline: deadline,
                Quiz: BuildQuizMetadata(quiz),
                Questions: shuffledQuestions
            )
        );
    }

    private static StartQuizMetadataResponse BuildQuizMetadata(QuizStartProjection quiz)
        => new(
            Id: quiz.Id,
            Title: quiz.Title,
            Instructions: quiz.Instructions,
            DurationMinutes: quiz.DurationMinutes,
            PassScore: quiz.PassScore,
            MaxAttempts: quiz.MaxAttempts
        );

    private static IReadOnlyCollection<StartQuizQuestionResponse> BuildShuffledQuestions(
        Guid attemptId,
        IReadOnlyCollection<QuestionStartProjection> questions)
    {
        return questions
            .OrderBy(question => GetStableOrder(attemptId, question.Id))
            .Select(question => new StartQuizQuestionResponse(
                Id: question.Id,
                Text: question.Text,
                Type: question.Type,
                Options: question.Options
                    .OrderBy(option => GetStableOrder(attemptId, option.Id))
                    .Select(option => new StartQuizQuestionOptionResponse(
                        Id: option.Id,
                        Text: option.Text
                    ))
                    .ToList()
            ))
            .ToList();
    }

    private static ulong GetStableOrder(Guid attemptId, Guid itemId)
    {
        Span<byte> combined = stackalloc byte[32];
        attemptId.TryWriteBytes(combined[..16]);
        itemId.TryWriteBytes(combined[16..]);

        Span<byte> hash = stackalloc byte[32];
        SHA256.TryHashData(combined, hash, out _);

        return BinaryPrimitives.ReadUInt64LittleEndian(hash[..8]);
    }

    private static bool IsInProgressAttemptConstraintViolation(DbUpdateException exception)
    {
        const string uniqueIndexName = "UX_QuizAttempts_UserId_QuizId_InProgress";
        var message = exception.InnerException?.Message ?? exception.Message;
        return message.Contains(uniqueIndexName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record QuizStartProjection(
        Guid Id,
        string Title,
        string Instructions,
        int DurationMinutes,
        int PassScore,
        int MaxAttempts,
        IReadOnlyCollection<QuestionStartProjection> Questions
    );

    private sealed record QuestionStartProjection(
        Guid Id,
        string Text,
        QuestionType Type,
        IReadOnlyCollection<QuestionOptionStartProjection> Options
    );

    private sealed record QuestionOptionStartProjection(
        Guid Id,
        string Text
    );
}
