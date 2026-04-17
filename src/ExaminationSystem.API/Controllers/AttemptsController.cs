using ExaminationSystem.Application.Features.Attempts.AnswerQuestion;
using ExaminationSystem.Application.Features.Attempts.SubmitAttempt;
using ExaminationSystem.Application.Features.Attempts.Timer;
using ExaminationSystem.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Claims;

namespace ExaminationSystem.API.Controllers;

[Route("api/attempts")]
// TODO: Uncomment when Identity setup is complete
//[Authorize]
public class AttemptsController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost("{attemptId:guid}/answer")]
    public async Task<IActionResult> Answer(Guid attemptId, AnswerQuestionOrchestrator command)
    {
        // TODO: replace StudentId to come from JWT claims once Identity is ready
        // var studentIdClaim = User.FindFirstValue("user_id");
        // if (studentIdClaim is null || !Guid.TryParse(studentIdClaim, out var parsedStudentId))
        //     return Unauthorized(
        //         ApiResponse<AnswerQuestionResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        // command = command with { StudentId = parsedStudentId };

        var result = await _mediator.Send(command with { AttemptId = attemptId });

        if (result.TimedOut)
            return StatusCode(410, ApiResponse<AnswerQuestionResponse>
                .Failure("Time has expired. Your attempt has been auto-submitted.", (HttpStatusCode)410));

        return Ok(ApiResponse<AnswerQuestionResponse>.Success(result));
    }

    [HttpPost("{attemptId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid attemptId, [FromQuery] Guid studentId)
    {
        // TODO: replace studentId parameter with JWT claim once Identity is ready
        // var studentIdClaim = User.FindFirstValue("user_id");
        // if (studentIdClaim is null || !Guid.TryParse(studentIdClaim, out var studentId))
        // {
        //     return Unauthorized(
        //         ApiResponse<SubmitAttemptResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        // }

        var result = await _mediator.Send(new SubmitAttemptOrchestrator(attemptId, studentId));
        if (result.TimedOut)
        {
            return StatusCode(410, new ApiResponse<SubmitAttemptResponse>(
                success: false,
                value: result.Value,
                errors: ["Time has expired. Your attempt has been auto-submitted."],
                statusCode: (HttpStatusCode)410
            ));
        }

        if (result.AlreadySubmitted)
        {
            return Conflict(new ApiResponse<SubmitAttemptResponse>(
                success: false,
                value: result.Value,
                errors: ["Attempt already submitted"],
                statusCode: HttpStatusCode.Conflict
            ));
        }

        return Ok(ApiResponse<SubmitAttemptResponse>.Success(result.Value));
    }

    [HttpGet("{attemptId:guid}/timer")]
    public async Task<IActionResult> Timer(Guid attemptId, [FromQuery] Guid studentId)
    {
        // TODO: replace studentId parameter with JWT claim once Identity is ready
        // var studentIdClaim = User.FindFirstValue("user_id");
        // if (studentIdClaim is null || !Guid.TryParse(studentIdClaim, out var studentId))
        // {
        //     return Unauthorized(
        //         ApiResponse<GetAttemptTimerResponse>.Failure("Invalid token claims.", HttpStatusCode.Unauthorized));
        // }

        var result = await _mediator.Send(new GetAttemptTimerQuery(attemptId, studentId));
        return Ok(ApiResponse<GetAttemptTimerResponse>.Success(result));
    }
}