using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Questions.CreateQuestion;

namespace ExaminationSystem.Application.Features.QuestionOptions.UpdateQuestionOptions;

public record UpdateQuestionOptionsCommand
(Guid QuestionId, List<QuestionOptionRequest> Options)
    : IQuery<RequestResult<bool>>;

public class UpdateQuestionOptionsCommandHandler(IUnitOfWork unitOfWork, IMediator mediator
    , ILogger<UpdateQuestionOptionsCommandHandler> logger)
    : IRequestHandler<UpdateQuestionOptionsCommand, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(UpdateQuestionOptionsCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.Repository<QuestionOption>();
        var options = request.Options.Select(o => new QuestionOption
        {
            QuestionId = request.QuestionId,
        }).ToList();
        foreach (var option in options) 
            repository.Delete(option);
       
        var saveResult = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult == 0)
        {
            logger.LogError("Failed to update options for question with id {Id}. No changes were saved to the database.", request.QuestionId);
            return RequestResult<bool>.Failure(false, ResultCode.QuestionFailedToUpdate);
        }
        logger.LogInformation("Options for question with id {Id} updated successfully.", request.QuestionId);
        return RequestResult<bool>.succeeded(true, ResultCode.QuestionUpdatedSuccessfully);
    }   
}