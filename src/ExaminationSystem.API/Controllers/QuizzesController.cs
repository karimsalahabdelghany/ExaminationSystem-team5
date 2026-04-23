using ExaminationSystem.Application.Features.Quizzes;
using ExaminationSystem.Application.Features.Quizzes.CreateQuiz;
using ExaminationSystem.Application.Features.Quizzes.DeleteQuiz;
using ExaminationSystem.Application.Features.Quizzes.PublishQuiz;
using ExaminationSystem.Application.Features.Quizzes.UnpublishQuiz;
using ExaminationSystem.Application.Features.Quizzes.UpdateQuiz;

namespace ExaminationSystem.API.Controllers;

[Route("api/admin/quizzes")]
// TODO: Uncomment when Identity setup is complete
//[Authorize(Roles = "Admin")]
public class QuizzesController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateQuizCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result is null)
            return BadRequest(ApiResponse<QuizResponse>.Failure("Can't create this quiz!"));
        return Created(
            $"/api/admin/quizzes/{result?.Result?.Id}",
            ApiResponse<QuizResponse>.Success(result?.Result, HttpStatusCode.Created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateQuizCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command with { QuizId = id }, cancellationToken);
        return result.Code switch
        {
            ResultCode.QuizNotFound => NotFound(ApiResponse<QuizResponse>.Failure("Quiz not found", HttpStatusCode.NotFound)),
            _ => Ok(ApiResponse<QuizResponse>.Success(result.Result, HttpStatusCode.OK))
        };
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteQuizCommand(id), cancellationToken);
        return result.Code switch
        {
            ResultCode.QuizNotFound => NotFound(ApiResponse<bool>.Failure("Quiz not found", HttpStatusCode.NotFound)),
            ResultCode.QuizIsPublished => Conflict(ApiResponse<bool>.Failure("Cannot delete a published quiz. Unpublish it first.", HttpStatusCode.Conflict)),
            ResultCode.QuizHasActiveAttempts => Conflict(ApiResponse<bool>.Failure("Cannot delete while active attempts exist", HttpStatusCode.Conflict)),
            _ => NoContent()
        };
    }


    [HttpPatch("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new PublishQuizCommand(id), cancellationToken);
        return result.Code switch
        {
            ResultCode.QuizNotFound => NotFound(ApiResponse<QuizResponse>.Failure("Quiz not found", HttpStatusCode.NotFound)),
            ResultCode.QuizAlreadyPublished => Conflict(ApiResponse<QuizResponse>.Failure("Quiz is already published", HttpStatusCode.Conflict)),
            ResultCode.QuizHasNoQuestions => UnprocessableEntity(ApiResponse<QuizResponse>.Failure("Quiz must have at least one question", HttpStatusCode.UnprocessableEntity)),
            _ => Ok(ApiResponse<QuizResponse>.Success(result.Result, HttpStatusCode.OK))
        };
    }


    [HttpPatch("{id:guid}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UnpublishQuizCommand(id), cancellationToken);
        return result.Code switch
        {
            ResultCode.QuizNotFound => NotFound(ApiResponse<QuizResponse>.Failure("Quiz not found", HttpStatusCode.NotFound)),
            ResultCode.QuizAlreadyDraft => Conflict(ApiResponse<QuizResponse>.Failure("Quiz is not currently published", HttpStatusCode.Conflict)),
            ResultCode.QuizHasActiveAttempts => Conflict(ApiResponse<QuizResponse>.Failure("Cannot unpublish while active attempts exist", HttpStatusCode.Conflict)),
            _ => Ok(ApiResponse<QuizResponse>.Success(result.Result, HttpStatusCode.OK))
        };
    }


}
