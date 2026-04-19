using ExaminationSystem.Application.Features.Quizzes;
using ExaminationSystem.Application.Features.Quizzes.CreateQuiz;
using ExaminationSystem.Application.Features.Quizzes.DeleteQuiz;
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
}
