
using ExaminationSystem.Application.Features.Diplomas.CreateDiploma;
using ExaminationSystem.Application.Responses;
using MediatR;
using System.Net;

namespace ExaminationSystem.API.Controllers;

public class DiplomasController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateDiplomaCommand command)
    {
        var result =await _mediator.Send(command);
        if (result is null)
            return BadRequest(ApiResponse<CreateDiplomaResponse>.Failure("Can't Create this diploma!"));
        return Created($"/Diplomas/{result.Id}", ApiResponse<CreateDiplomaResponse>.Success(result,HttpStatusCode.Created));
    }
}
