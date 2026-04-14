using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Application.Features.Attempts.Commands;
using ExaminationSystem.Application.Features.Attempts.Queries;
using ExaminationSystem.Application.Features.Questions.Queries;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Attempts.AnswerQuestion;

public record AnswerQuestionOrchestrator(
    Guid AttemptId,
    Guid QuestionId,
    Guid SelectedOptionId,
    Guid StudentId
) : ICommand<AnswerQuestionResponse>;


public class AnswerQuestionOrchestratorHandler(
    IMediator mediator) : IRequestHandler<AnswerQuestionOrchestrator, AnswerQuestionResponse>
{
    public async Task<AnswerQuestionResponse> Handle(
        AnswerQuestionOrchestrator request, CancellationToken cancellationToken)
    {
        var attempt = await mediator.Send(
            new GetAttemptByIdQuery(request.AttemptId), cancellationToken);

        if (attempt is null)
            throw new NotFoundException("Attempt", request.AttemptId);

        if (attempt.UserId != request.StudentId)
            throw new ForbiddenException("You do not own this attempt.");

        if (attempt.Status != QuizAttemptStatus.InProgress)
            throw new ConflictException("Attempt", "Attempt is already submitted or expired.");

        if (DateTime.UtcNow > attempt.Deadline)
        {
            await mediator.Send(
                new MarkAttemptTimedOutCommand(request.AttemptId), cancellationToken);

            return new AnswerQuestionResponse(Saved: false, TimedOut: true);
        }

        var questionBelongsToQuiz = await mediator.Send(
            new IsQuestionInQuizQuery(request.QuestionId, attempt.QuizId), cancellationToken);

        if (!questionBelongsToQuiz)
            throw new UnprocessableException("This question does not belong to this quiz.");

        await mediator.Send(new UpsertAnswerCommand(
            request.AttemptId, request.QuestionId, request.SelectedOptionId
        ), cancellationToken);

        return new AnswerQuestionResponse(Saved: true);
    }
}
