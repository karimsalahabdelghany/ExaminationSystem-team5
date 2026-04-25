using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.QuestionOptions.CreateQuestionOptions;
using ExaminationSystem.Application.Features.QuestionOptions.DeleteAllOptionsForQuestion;
using ExaminationSystem.Application.Features.Questions.CheckQuestionExist;
using ExaminationSystem.Application.Features.Questions.CreateQuestion;
using ExaminationSystem.Application.Features.Questions.UpdateQuestionBasicInfo;
using System.Data;

namespace ExaminationSystem.Application.Features.Questions.UpdateQuestion;

public record UpdateQuestionOrchestrator
    (Guid Id, string Text, string Explanation, List<QuestionOptionRequest> Options)
    : ICommand<RequestResult<bool>>;

public class UpdateQuestionOrchestratorHandler(IUnitOfWork unitOfWork, IMediator mediator
    , ILogger<UpdateQuestionOrchestratorHandler> logger)
    : IRequestHandler<UpdateQuestionOrchestrator, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(UpdateQuestionOrchestrator request, CancellationToken cancellationToken)
    {
        var hasOneCorrectAnswer = CheckOptionsHasOnlyOneCorrectAnswer(request.Options);
        if (!hasOneCorrectAnswer)
            return RequestResult<bool>.Failure(false, ResultCode.QuestionHasMoreThanOneCorrectAnswer);

        var isQuestionExist = await mediator.Send(new CheckQuestionExistQuery(request.Id), cancellationToken);
        if (!isQuestionExist.Result)
        {
            logger.LogWarning("Question with id {Id} does not exist.", request.Id);
            return RequestResult<bool>.Failure(false, ResultCode.QuestionNotFound);
        }
        await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        var updateBasicInfoResult = await mediator
            .Send(new UpdateQuestionBasicInfoCommand(request.Id, request.Text, request.Explanation),
            cancellationToken);

        if (!updateBasicInfoResult.Result)
        {
            logger.LogError("Failed to update basic info for question with id {Id}.", request.Id);
            await unitOfWork.RollbackAsync(cancellationToken);
            return RequestResult<bool>.Failure(false, ResultCode.QuestionFailedToUpdate);
        }

        var deleteOldOptionsResult = await mediator
            .Send(new DeleteAllOptionsForQuestionCommand(request.Id),
            cancellationToken);

        if (!deleteOldOptionsResult.Result)
        {
            logger.LogError("Failed to delete old options for question with id {Id}.", request.Id);
            await unitOfWork.RollbackAsync(cancellationToken);
            return RequestResult<bool>.Failure(false, ResultCode.QuestionFailedToUpdate);
        }

        var addNewOptionsToQuestionResult = await mediator
            .Send(new CreateQuestionOptionsCommand(request.Id, request.Options),
            cancellationToken);
        if (!addNewOptionsToQuestionResult.Result)
        {
            logger.LogError("Failed to add new options for question with id {Id}.", request.Id);
            await unitOfWork.RollbackAsync(cancellationToken);
            return RequestResult<bool>.Failure(false, ResultCode.QuestionFailedToUpdate);
        }


        try
        {
            await unitOfWork.CommitAsync(cancellationToken);
            logger.LogInformation("Question with id {Id} updated successfully.", request.Id);
            return RequestResult<bool>.succeeded(true, ResultCode.QuestionUpdatedSuccessfully);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while committing the transaction for updating question with id {Id}.", request.Id);
            await unitOfWork.RollbackAsync(cancellationToken);
            return RequestResult<bool>.Failure(false, ResultCode.QuestionFailedToUpdate);

        }
    }

    private bool CheckOptionsHasOnlyOneCorrectAnswer(List<QuestionOptionRequest> options)
    {
        return options.Count(o => o.IsCorrect) == 1;
    }
}       
        