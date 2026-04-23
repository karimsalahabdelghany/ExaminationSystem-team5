using ExaminationSystem.Application.Common.Results;
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
                ReqestResult<StartQuizAttemptResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        }

        var result = await _mediator.Send(new StartQuizAttemptCommand(id, studentId));

        if (result.Success)
        {
            return Ok(ReqestResult<StartQuizAttemptResponse>.Success(result.Result));
        }

        return result.Code switch
        {
            ResultCode.AttemptAlreadyInProgress => Conflict(new ReqestResult<StartQuizAttemptResponse>(
                success: false,
                value: result.Result,
                errors: ["Attempt already in progress"],
                statusCode: HttpStatusCode.Conflict
            )),
            ResultCode.QuizNotFoundOrNotPublished => NotFound(
                ReqestResult<StartQuizAttemptResponse>.Failure(
                    "Quiz not found or not published.",
                    HttpStatusCode.NotFound)),
            ResultCode.AttemptLimitReached => StatusCode(
                StatusCodes.Status403Forbidden,
                ReqestResult<StartQuizAttemptResponse>.Failure(
                    "Attempt limit reached",
                    HttpStatusCode.Forbidden)),
            ResultCode.AttemptStartConflict => Conflict(
                ReqestResult<StartQuizAttemptResponse>.Failure(
                    "Attempt already in progress",
                    HttpStatusCode.Conflict)),
            _ => BadRequest(
                ReqestResult<StartQuizAttemptResponse>.Failure("Could not start quiz attempt."))
        };
    }
}
