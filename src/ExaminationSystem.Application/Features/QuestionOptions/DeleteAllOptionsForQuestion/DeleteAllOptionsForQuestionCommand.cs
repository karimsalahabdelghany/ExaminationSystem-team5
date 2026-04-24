using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.QuestionOptions.DeleteAllOptionsForQuestion;

public record DeleteAllOptionsForQuestionCommand
(Guid QuestionId) : ICommand<RequestResult<bool>>;

public class DeleteAllOptionsForQuestionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    : IRequestHandler<DeleteAllOptionsForQuestionCommand, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(DeleteAllOptionsForQuestionCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<QuestionOption>();
        var optionsId = await repository.GetAll(o => o.QuestionId == request.QuestionId)
                                        .Select(o => o.Id) 
                                        .ToListAsync(cancellationToken);
        if (optionsId.Count == 0)
            return RequestResult<bool>.Failure(false, ResultCode.OptionsNotFound);
        //Ask For Excute Delete here 
        foreach (var optionId in optionsId)
        {
            var option = new QuestionOption { Id = optionId };
            repository.Delete(option);
        }

        return RequestResult<bool>.succeeded(true, ResultCode.OptionDeletedSuccessfully);
    }
}

