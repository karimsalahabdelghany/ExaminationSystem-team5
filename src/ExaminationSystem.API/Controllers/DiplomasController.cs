
using ExaminationSystem.Application.Features.Diplomas.CreateDiploma;
using ExaminationSystem.Application.Features.Diplomas.UpdateDiploma;
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateDiplomaCommand command)
    {
        var result = await _mediator.Send(command);
        if (result is null)
            return BadRequest(ApiResponse<UpdateDiplomaResult>.Failure("Can't update this diploma!"));
        return Ok(ApiResponse<UpdateDiplomaResult>.Success(result, HttpStatusCode.OK));
    }
}
