using ExaminationSystem.Application.Features.Attempts.AnswerQuestion;
using ExaminationSystem.Application.Features.Attempts.SubmitAttempt;
using ExaminationSystem.Application.Features.Attempts.Timer;

namespace ExaminationSystem.API.Controllers;

[Route("api/attempts")]
// TODO: Uncomment when Identity setup is complete
//[Authorize]
public class AttemptsController(IMediator mediator) : BaseController(mediator)
{

    [HttpPost("{attemptId:guid}/answer")]
    public async Task<IActionResult> Answer(Guid attemptId, AnswerQuestionOrchestrator command, CancellationToken cancellationToken)
    {
        // TODO: replace StudentId to come from JWT claims once Identity is ready
        // var studentIdClaim = User.FindFirstValue("user_id");
        // if (studentIdClaim is null || !Guid.TryParse(studentIdClaim, out var parsedStudentId))
        //     return Unauthorized(
        //         ApiResponse<AnswerQuestionResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        // command = command with { StudentId = parsedStudentId };

        var result = await _mediator.Send(command with { AttemptId = attemptId }, cancellationToken);

        return result.Code switch
        {
            ResultCode.AttemptNotFound => NotFound(ApiResponse<AnswerQuestionResponse>.Failure("Attempt not found", HttpStatusCode.NotFound)),
            ResultCode.AttemptNotOwned => Forbid("You do not own this attempt"),
            ResultCode.AttemptAlreadySubmitted => Conflict(ApiResponse<AnswerQuestionResponse>.Failure("Attempt is already submitted or expired", HttpStatusCode.Conflict)),
            ResultCode.AttemptTimedOut => StatusCode(410, ApiResponse<AnswerQuestionResponse>.Failure("Time has expired. Your attempt has been auto-submitted", (HttpStatusCode)410)),
            ResultCode.QuestionNotInQuiz => UnprocessableEntity(ApiResponse<AnswerQuestionResponse>.Failure("This question does not belong to this quiz", HttpStatusCode.UnprocessableEntity)),
            _ => Ok(ApiResponse<AnswerQuestionResponse>.Success(result.Result, HttpStatusCode.OK))
        };
    }

    [HttpPost("{attemptId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid attemptId)
    {
        var studentIdClaim = User.FindFirstValue("user_id");
        if (studentIdClaim is null || !Guid.TryParse(studentIdClaim, out var studentId))
        {
            return Unauthorized(
                ApiResponse<SubmitAttemptResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        }

        var result = await _mediator.Send(new SubmitAttemptOrchestrator(attemptId, studentId));

        if (result.Success)
            return Ok(ApiResponse<SubmitAttemptResponse>.Success(result.Result));

        return result.Code switch
        {
            ResultCode.AttemptTimedOut => StatusCode(410, new ApiResponse<SubmitAttemptResponse>(
                success: false,
                value: result.Result,
                errors: ["Time has expired. Your attempt has been auto-submitted."],
                statusCode: (HttpStatusCode)410
            )),
            ResultCode.AttemptAlreadySubmitted => Conflict(new ApiResponse<SubmitAttemptResponse>(
                success: false,
                value: result.Result,
                errors: ["Attempt already submitted"],
                statusCode: HttpStatusCode.Conflict
            )),
            ResultCode.AttemptNotFound => NotFound(
                ApiResponse<SubmitAttemptResponse>.Failure("Attempt not found", HttpStatusCode.NotFound)),
            ResultCode.AttemptNotOwned => Forbid("You do not own this attempt."),
            _ => BadRequest(ApiResponse<SubmitAttemptResponse>.Failure("Could not submit attempt."))
        };
    }


    [HttpGet("{attemptId:guid}/timer")]
    public async Task<IActionResult> Timer(Guid attemptId)
    {
        var studentIdClaim = User.FindFirstValue("user_id");
        if (studentIdClaim is null || !Guid.TryParse(studentIdClaim, out var studentId))
        {
            return Unauthorized(
                ApiResponse<GetAttemptTimerResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        }

        var result = await _mediator.Send(new GetAttemptTimerQuery(attemptId, studentId));
        return Ok(ApiResponse<GetAttemptTimerResponse>.Success(result));
    }
}
