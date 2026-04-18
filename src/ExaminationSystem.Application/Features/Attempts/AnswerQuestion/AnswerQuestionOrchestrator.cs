using ExaminationSystem.Application.Common.Results;
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
) : ICommand<RequestResult<AnswerQuestionResponse>>;


public class AnswerQuestionOrchestratorHandler(
    IMediator mediator,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<AnswerQuestionOrchestrator, RequestResult<AnswerQuestionResponse>>
{
    public async Task<RequestResult<AnswerQuestionResponse>> Handle(
        AnswerQuestionOrchestrator request, CancellationToken cancellationToken)
    {
        var attempt = await mediator.Send(
            new GetAttemptByIdQuery(request.AttemptId), cancellationToken);

        var existsCheck = CheckAttemptExists(attempt);
        if (existsCheck is not null) return existsCheck;

        var ownershipCheck = CheckAttemptOwnership(attempt!, request.StudentId);
        if (ownershipCheck is not null) return ownershipCheck;

        if (IsAttemptExpired(attempt!))
            return TimedOutResult();

        var progressCheck = CheckAttemptIsInProgress(attempt!);
        if (progressCheck is not null) return progressCheck;

        if (dateTimeProvider.UtcNow > attempt!.Deadline)
        {
            await mediator.Send(new SubmitAttemptOrchestrator(
                AttemptId: request.AttemptId,
                StudentId: request.StudentId
            ), cancellationToken);

            return TimedOutResult();
        }

        var questionBelongsToQuiz = await mediator.Send(
            new IsQuestionInQuizQuery(request.QuestionId, attempt.QuizId), cancellationToken);

        var questionCheck = CheckQuestionBelongsToQuiz(questionBelongsToQuiz);
        if (questionCheck is not null) return questionCheck;

        await mediator.Send(new UpsertAnswerCommand(
            request.AttemptId, request.QuestionId, request.SelectedOptionId
        ), cancellationToken);

        return RequestResult<AnswerQuestionResponse>.succeeded(
            new AnswerQuestionResponse(Saved: true),
            ResultCode.AnswerSavedSuccessfully);
    }




    private static RequestResult<AnswerQuestionResponse>? CheckAttemptExists(QuizAttempt? attempt)
    {
        return attempt is null
            ? RequestResult<AnswerQuestionResponse>.Failure(null!, ResultCode.AttemptNotFound)
            : null;
    }

    private static RequestResult<AnswerQuestionResponse>? CheckAttemptOwnership(QuizAttempt attempt, Guid studentId)
    {
        return attempt.UserId != studentId
            ? RequestResult<AnswerQuestionResponse>.Failure(null!, ResultCode.AttemptNotOwned)
            : null;
    }

    private static bool IsAttemptExpired(QuizAttempt attempt)
        => attempt.Status == QuizAttemptStatus.Expired;

    private static RequestResult<AnswerQuestionResponse>? CheckAttemptIsInProgress(QuizAttempt attempt)
    {
        return attempt.Status != QuizAttemptStatus.InProgress
            ? RequestResult<AnswerQuestionResponse>.Failure(null!, ResultCode.AttemptAlreadySubmitted)
            : null;
    }

    private static RequestResult<AnswerQuestionResponse>? CheckQuestionBelongsToQuiz(bool belongs)
    {
        return !belongs
            ? RequestResult<AnswerQuestionResponse>.Failure(null!, ResultCode.QuestionNotInQuiz)
            : null;
    }

    private static RequestResult<AnswerQuestionResponse> TimedOutResult()
        => RequestResult<AnswerQuestionResponse>.Failure(
            new AnswerQuestionResponse(Saved: false, TimedOut: true),
            ResultCode.AttemptTimedOut);
}
