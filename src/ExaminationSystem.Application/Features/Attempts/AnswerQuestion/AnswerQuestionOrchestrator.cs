using ExaminationSystem.Application.Features.Attempts.Queries;
using ExaminationSystem.Application.Features.Attempts.SubmitAttempt;
using ExaminationSystem.Application.Features.Questions.Queries;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Attempts.AnswerQuestion;

public record AnswerQuestionOrchestrator(
    Guid AttemptId,
    Guid QuestionId,
    Guid SelectedOptionId,
    Guid StudentId
) : ICommand<AnswerQuestionResponse>;


public class AnswerQuestionOrchestratorHandler(
    IMediator mediator,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<AnswerQuestionOrchestrator, AnswerQuestionResponse>
{
    public async Task<AnswerQuestionResponse> Handle(
        AnswerQuestionOrchestrator request, CancellationToken cancellationToken)
    {
        var attempt = await mediator.Send(
            new GetAttemptByIdQuery(request.AttemptId), cancellationToken);

        EnsureAttemptExists(attempt, request.AttemptId);
        EnsureAttemptOwnership(attempt!, request.StudentId);

        if (IsAttemptExpired(attempt!))
            return new AnswerQuestionResponse(Saved: false, TimedOut: true);

        EnsureAttemptIsInProgress(attempt!);

        if (dateTimeProvider.UtcNow > attempt!.Deadline)
        {
            await mediator.Send(new SubmitAttemptOrchestrator(
                AttemptId: request.AttemptId,
                StudentId: request.StudentId
            ), cancellationToken);

            return new AnswerQuestionResponse(Saved: false, TimedOut: true);
        }

        var questionBelongsToQuiz = await mediator.Send(
            new IsQuestionInQuizQuery(request.QuestionId, attempt.QuizId), cancellationToken);

        EnsureQuestionBelongsToQuiz(questionBelongsToQuiz);

        await mediator.Send(new UpsertAnswerCommand(
            request.AttemptId, request.QuestionId, request.SelectedOptionId
        ), cancellationToken);

        return new AnswerQuestionResponse(Saved: true);
    }





    private static void EnsureAttemptExists(QuizAttempt? attempt, Guid attemptId)
    {
        if (attempt is null)
            throw new NotFoundException("Attempt", attemptId);
    }

    private static void EnsureAttemptOwnership(QuizAttempt attempt, Guid studentId)
    {
        if (attempt.UserId != studentId)
            throw new ForbiddenException("You do not own this attempt.");
    }

    private static bool IsAttemptExpired(QuizAttempt attempt)
        => attempt.Status == QuizAttemptStatus.Expired;

    private static void EnsureAttemptIsInProgress(QuizAttempt attempt)
    {
        if (attempt.Status != QuizAttemptStatus.InProgress)
            throw new ConflictException("Attempt", "Attempt is already submitted or expired.");
    }

    private static void EnsureQuestionBelongsToQuiz(bool belongs)
    {
        if (!belongs)
            throw new UnprocessableException("This question does not belong to this quiz.");
    }
}
