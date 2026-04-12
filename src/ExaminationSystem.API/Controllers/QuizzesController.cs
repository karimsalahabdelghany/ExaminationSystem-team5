using ExaminationSystem.Application.Features.Quizzes.CreateQuiz;
using ExaminationSystem.Application.Features.Quizzes;
using ExaminationSystem.Application.Responses;
using MediatR;
using System.Net;

namespace ExaminationSystem.API.Controllers;

[Route("api/admin/quizzes")]
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


}
