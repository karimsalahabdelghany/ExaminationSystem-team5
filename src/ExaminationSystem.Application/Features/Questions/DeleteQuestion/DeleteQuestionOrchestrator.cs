using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.QuestionOptions.DeleteAllOptionsForQuestion;
using ExaminationSystem.Application.Features.Questions.CheckQuestionExistAndQuizPuplished;

namespace ExaminationSystem.Application.Features.Questions.DeleteQuestion;

public record DeleteQuestionOrchestrator(Guid Id) : IQuery<RequestResult<bool>>;

public class DeleteQuestionOrchestratorHandler(IUnitOfWork unitOfWork, IMediator mediator, ILogger<DeleteQuestionOrchestratorHandler> logger)
    : IRequestHandler<DeleteQuestionOrchestrator, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(DeleteQuestionOrchestrator request, CancellationToken cancellationToken)
    {
        var isQuestionExist = await mediator.Send(new CheckQuestionExistAndQuizNotPuplishedQuery(request.Id), cancellationToken);
        if (!isQuestionExist.Result)
        {
            logger.LogWarning("Question with id {Id} does not exist or quiz is published.", request.Id);
            return RequestResult<bool>.Failure(false, ResultCode.QuestionNotFoundOrQuizPublished);
        }
        await unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted,cancellationToken);
        var questionRepository = unitOfWork.Repository<Question>();
        questionRepository.Delete(new Question { Id = request.Id });
        var deletedOptions = await mediator.Send(new DeleteAllOptionsForQuestionCommand(request.Id), cancellationToken);
        if (!deletedOptions.Result )
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            logger.LogError("Failed to delete options for question with id {Id}. Rolling back transaction.", request.Id);
            return RequestResult<bool>.Failure(false, ResultCode.QuestionDeleteFailed);
        }
        try
        {
            await unitOfWork.CommitAsync(cancellationToken);
                logger.LogInformation("Question with id {Id} deleted successfully.", request.Id);
            return RequestResult<bool>.succeeded(true, ResultCode.QuestionDeletedSuccessfully);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Failed to delete question with id {Id}. Rolling back transaction.", request.Id);
            return RequestResult<bool>.Failure(false, ResultCode.QuestionDeleteFailed);
        }
    }
}
