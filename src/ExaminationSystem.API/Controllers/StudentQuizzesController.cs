using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Quizzes.StartQuizAttempt;
using ExaminationSystem.Application.Interfaces;

namespace ExaminationSystem.API.Controllers;

[Route("api/quizzes")]
[Authorize(Roles = "Student")]
public class StudentQuizzesController(IMediator mediator, ICurrentUser currentUser) : BaseController(mediator)
{
    private readonly ICurrentUser _currentUser = currentUser;

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id)
    {
        if (!_currentUser.TryGetUserId(out var studentId))
        {
            return Unauthorized(
                ApiResponse<StartQuizAttemptResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        }

        var result = await _mediator.Send(new StartQuizAttemptCommand(id, studentId));

        if (result.Success)
        {
            return Ok(ApiResponse<StartQuizAttemptResponse>.Success(result.Result));
        }

        return result.Code switch
        {
            ResultCode.AttemptAlreadyInProgress => Conflict(new ApiResponse<StartQuizAttemptResponse>(
                success: false,
                value: result.Result,
                errors: ["Attempt already in progress"],
                statusCode: HttpStatusCode.Conflict
            )),
            ResultCode.QuizNotFoundOrNotPublished => NotFound(
                ApiResponse<StartQuizAttemptResponse>.Failure(
                    "Quiz not found or not published.",
                    HttpStatusCode.NotFound)),
            ResultCode.AttemptLimitReached => StatusCode(
                StatusCodes.Status403Forbidden,
                ApiResponse<StartQuizAttemptResponse>.Failure(
                    "Attempt limit reached",
                    HttpStatusCode.Forbidden)),
            ResultCode.AttemptStartConflict => Conflict(
                ApiResponse<StartQuizAttemptResponse>.Failure(
                    "Attempt already in progress",
                    HttpStatusCode.Conflict)),
            _ => BadRequest(
                ApiResponse<StartQuizAttemptResponse>.Failure("Could not start quiz attempt."))
        };
    }
}
