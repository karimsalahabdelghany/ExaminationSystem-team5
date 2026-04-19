using ExaminationSystem.Application.Features.Quizzes.StartQuizAttempt;

namespace ExaminationSystem.API.Controllers;

[Route("api/quizzes")]
[Authorize(Roles = "Student")]
public class StudentQuizzesController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id)
    {
        var studentIdClaim = User.FindFirstValue("user_id");
        if (studentIdClaim is null || !Guid.TryParse(studentIdClaim, out var studentId))
        {
            return Unauthorized(
                ApiResponse<StartQuizAttemptResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        }

        var result = await _mediator.Send(new StartQuizAttemptCommand(id, studentId));
        if (result.Value is null)
        {
            return BadRequest(
                ApiResponse<StartQuizAttemptResponse>.Failure("Could not start quiz attempt."));
        }

        if (result.HasInProgressAttempt)
        {
            return Conflict(new ApiResponse<StartQuizAttemptResponse>(
                success: false,
                value: result.Value,
                errors: ["Attempt already in progress"],
                statusCode: HttpStatusCode.Conflict
            ));
        }

        return Ok(ApiResponse<StartQuizAttemptResponse>.Success(result.Value));
    }
}
