using ExaminationSystem.Application.Features.Attempts.AnswerQuestion;
using ExaminationSystem.Application.Features.Attempts.SubmitAttempt;
using ExaminationSystem.Application.Features.Attempts.Timer;
using ExaminationSystem.Application.Interfaces;

namespace ExaminationSystem.API.Controllers;

[Route("api/attempts")]
// TODO: Uncomment when Identity setup is complete
//[Authorize]
public class AttemptsController(IMediator mediator,ICurrentUser currentUser) : BaseController(mediator)
{
    private readonly ICurrentUser _currentUser = currentUser;

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
            ResultCode.AttemptNotFound => NotFound(ReqestResult<AnswerQuestionResponse>.Failure("Attempt not found", HttpStatusCode.NotFound)),
            ResultCode.AttemptNotOwned => Forbid("You do not own this attempt"),
            ResultCode.AttemptAlreadySubmitted => Conflict(ReqestResult<AnswerQuestionResponse>.Failure("Attempt is already submitted or expired", HttpStatusCode.Conflict)),
            ResultCode.AttemptTimedOut => StatusCode(410, ReqestResult<AnswerQuestionResponse>.Failure("Time has expired. Your attempt has been auto-submitted", (HttpStatusCode)410)),
            ResultCode.QuestionNotInQuiz => UnprocessableEntity(ReqestResult<AnswerQuestionResponse>.Failure("This question does not belong to this quiz", HttpStatusCode.UnprocessableEntity)),
            _ => Ok(ReqestResult<AnswerQuestionResponse>.Success(result.Result, HttpStatusCode.OK))
        };
    }

    [HttpPost("{attemptId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid attemptId)
    {
        var studentIdClaim = _currentUser.Id;
        if (studentIdClaim is null)
        {
            return Unauthorized(
                ReqestResult<SubmitAttemptResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        }

        var result = await _mediator.Send(new SubmitAttemptOrchestrator(attemptId, _currentUser.Id.Value));

        if (result.Success)
            return Ok(ReqestResult<SubmitAttemptResponse>.Success(result.Result));

        return result.Code switch
        {
            ResultCode.AttemptTimedOut => StatusCode(410, new ReqestResult<SubmitAttemptResponse>(
                success: false,
                value: result.Result,
                errors: ["Time has expired. Your attempt has been auto-submitted."],
                statusCode: (HttpStatusCode)410
            )),
            ResultCode.AttemptAlreadySubmitted => Conflict(new ReqestResult<SubmitAttemptResponse>(
                success: false,
                value: result.Result,
                errors: ["Attempt already submitted"],
                statusCode: HttpStatusCode.Conflict
            )),
            ResultCode.AttemptNotFound => NotFound(
                ReqestResult<SubmitAttemptResponse>.Failure("Attempt not found", HttpStatusCode.NotFound)),
            ResultCode.AttemptNotOwned => Forbid("You do not own this attempt."),
            _ => BadRequest(ReqestResult<SubmitAttemptResponse>.Failure("Could not submit attempt."))
        };
    }


    [HttpGet("{attemptId:guid}/timer")]
    public async Task<IActionResult> Timer(Guid attemptId)
    {
        var studentIdClaim = _currentUser.Id;
        if (studentIdClaim is null )
        {
            return Unauthorized(
                ReqestResult<GetAttemptTimerResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        }

        var result = await _mediator.Send(new GetAttemptTimerQuery(attemptId,_currentUser.Id.Value));
        return Ok(ReqestResult<GetAttemptTimerResponse>.Success(result));
    }
    

}
