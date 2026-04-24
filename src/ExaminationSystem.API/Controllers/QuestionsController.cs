using ExaminationSystem.Application.Features.Questions.CreateQuestion;
using ExaminationSystem.Application.Features.Questions.DeleteQuestion;
using ExaminationSystem.Application.Features.Questions.UpdateQuestion;

namespace ExaminationSystem.API.Controllers;

public class QuestionsController(IMediator mediator) : BaseController(mediator)
{
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateQuestionOrchestrator command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command with { Id = id }, cancellationToken);
        return result.Code switch
        {
            ResultCode.QuestionNotFound => NotFound(ApiResponse<bool>.Failure("Question not found", HttpStatusCode.NotFound)),
            ResultCode.QuestionUpdatedSuccessfully => Ok(ApiResponse<bool>.Success(true, HttpStatusCode.OK)),
            ResultCode.QuestionHasMoreThanOneCorrectAnswer => UnprocessableEntity(ApiResponse<bool>.Failure("Exactly one correct option required", HttpStatusCode.UnprocessableEntity)),
            _ => BadRequest(ApiResponse<bool>.Failure("Failed to update question", HttpStatusCode.BadRequest))
        };
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteQuestionOrchestrator(id), cancellationToken);
        return result.Code switch
        {
            ResultCode.QuestionNotFound => NotFound(ApiResponse<bool>.Failure("Question not found", HttpStatusCode.NotFound)),
            ResultCode.QuestionDeletedSuccessfully => Ok(ApiResponse<bool>.Success(true, HttpStatusCode.OK)),
            ResultCode.QuestionNotFoundOrQuizPublished => Conflict(ApiResponse<bool>.Failure("Cannot delete question from a published quiz", HttpStatusCode.Conflict)),
            _ => BadRequest(ApiResponse<bool>.Failure("Failed to delete question", HttpStatusCode.BadRequest))
        };
    }

}
