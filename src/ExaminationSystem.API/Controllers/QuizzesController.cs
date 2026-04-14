using ExaminationSystem.Application.Features.Quizzes;
using ExaminationSystem.Application.Features.Quizzes.CreateQuiz;
using ExaminationSystem.Application.Features.Quizzes.DeleteQuiz;
using ExaminationSystem.Application.Features.Quizzes.UpdateQuiz;
using ExaminationSystem.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace ExaminationSystem.API.Controllers;

[Route("api/admin/quizzes")]
// TODO: Uncomment when Identity setup is complete
//[Authorize(Roles = "Admin")]
public class QuizzesController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateQuizCommand command)
    {
        var result = await _mediator.Send(command);
        return Created(
            $"/api/admin/quizzes/{result.Id}",
            ApiResponse<QuizResponse>.Success(result, HttpStatusCode.Created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateQuizCommand command)
    {
        var result = await _mediator.Send(command with { QuizId = id });
        return Ok(ApiResponse<QuizResponse>.Success(result));
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteQuizCommand(id));
        return NoContent();
    }
}


