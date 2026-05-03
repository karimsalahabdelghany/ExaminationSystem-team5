using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Diplomas.CheckDiplomaExist;
using ExaminationSystem.Application.Features.Diplomas.CheckDiplomaHasActiveEnrollmentsOrPublished;
using ExaminationSystem.Application.Features.Questions.DeleteDiplomaQuizQuestionsByDiplomaId;
using ExaminationSystem.Application.Features.Quizzes.DeleteDiplomaQuizzesByDiplomaId;
using System.Data;

namespace ExaminationSystem.Application.Features.Diplomas.DeleteDiploma;

public record DeleteDiplomaOrchestrator(Guid Id) : ICommand<RequestResult<bool>>;

public class DeleteDiplomaOrchestratorHandler(IUnitOfWork unitOfWork ,IMediator mediator) : IRequestHandler<DeleteDiplomaOrchestrator, RequestResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;

    public async Task<RequestResult<bool>> Handle(DeleteDiplomaOrchestrator request, CancellationToken cancellationToken)
    {
        var _repository = _unitOfWork.Repository<Diploma>();
        var isExist = await _mediator.Send(new CheckDiplomaExistQuery(request.Id), cancellationToken);
        if (!isExist.Result)
            return RequestResult<bool>.Failure(false, ResultCode.DiplomaNotFound);

        var hasActiveEnrollmentsOrPublished = await _mediator.Send(new CheckDiplomaHasActiveEnrollmentsOrPublishedQuery(request.Id), cancellationToken);
        if (hasActiveEnrollmentsOrPublished.Result)
            return RequestResult<bool>.Failure(false,ResultCode.DiplomaHasActiveEnrollmentsOrPublished);
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        _repository.Delete(new Diploma
        {
            Id = request.Id
        });
        var deleteQuizeResult = await _mediator.Send(new DeleteDiplomaQuizzesByDiplomaIdCommand(request.Id), cancellationToken); 
        if (!deleteQuizeResult.Result)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return RequestResult<bool>.Failure(false, deleteQuizeResult.Code);
        }
        var deleteQuizeQuestionsResult = await _mediator.Send(new DeleteDiplomaQuizQuestionsByDiplomaIdCommand(request.Id), cancellationToken);
        if (!deleteQuizeQuestionsResult.Result)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return RequestResult<bool>.Failure(false, deleteQuizeQuestionsResult.Code);
        }
        try
        {
            await _unitOfWork.CommitAsync(cancellationToken);
            return RequestResult<bool>.succeeded(true, ResultCode.DiplomaDeletedSuccessfully);

        }catch(Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return RequestResult<bool>.Failure(false, ResultCode.DiplomaDeletedSuccessfully);
        }
    }
}

