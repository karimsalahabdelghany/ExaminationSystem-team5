
using ExaminationSystem.Application.Features.Attempts.AnswerQuestion;
using ExaminationSystem.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ExaminationSystem.API.Controllers;

[Route("api/attempts")]
// TODO: Uncomment
// [Authorize]
public class AttemptsController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost("{attemptId:guid}/answer")]
    public async Task<IActionResult> Answer(Guid attemptId, AnswerQuestionRequest request)
    {
        // TODO: uncomment, will be replaced with actual user claim when Identity is ready
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
        //var parsedStudentId = Guid.TryParse(studentId, out var sid) ? sid : Guid.Empty; 

        //TODO: remove
        var parsedStudentId = Guid.Parse("88888888-8888-8888-8888-888888888888");

        var result = await _mediator.Send(new AnswerQuestionOrchestrator(
            AttemptId: attemptId,
            QuestionId: request.QuestionId,
            SelectedOptionId: request.SelectedOptionId,
            StudentId: parsedStudentId
        ));

        return Ok(ApiResponse<AnswerQuestionResponse>.Success(result));
    }
}

public record AnswerQuestionRequest(Guid QuestionId, Guid SelectedOptionId);
