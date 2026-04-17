using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Diplomas.CreateDiploma;
using ExaminationSystem.Application.Features.Diplomas.DeleteDiploma;
using ExaminationSystem.Application.Features.Diplomas.GetDiplomaQuizez;
using ExaminationSystem.Application.Features.Diplomas.GetDiplomas;
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
        return Created($"/Diplomas/{result.Result.Id}", ApiResponse<CreateDiplomaResponse>.Success(result.Result,HttpStatusCode.Created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateDiplomaCommand command)
    {
        var result = await _mediator.Send(command);
        if (result is null)
            return BadRequest(ApiResponse<UpdateDiplomaResult>.Failure("Can't update this diploma!"));
        return Ok(ApiResponse<UpdateDiplomaResult>.Success(result.Result, HttpStatusCode.OK));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteDiplomaCommand(id));
        if (!result.Success && result.Code == ResultCode.DiplomaNotFound)
            return NotFound(ApiResponse<bool>.Failure("Diploma not found", HttpStatusCode.NotFound));
        if (!result.Success && result.Code == ResultCode.DiplomaHasActiveEnrollmentsOrPublished)
            return Conflict(ApiResponse<bool>.Failure("Can't delete this diploma because it has active enrollments or is published", HttpStatusCode.Conflict));
        return Ok(ApiResponse<bool>.Success(true, HttpStatusCode.OK));
    }

    [HttpGet]
    public async Task<IActionResult> GetDiplomas([FromQuery] GetDiplomasQuery query)
    {
        var result = await _mediator.Send(query);
        if (result is null)
            return NotFound(ApiResponse<PaginationResult<GetDiplomaResponse>>.Failure("No diplomas found"));
        return Ok(ApiResponse<PaginationResult<GetDiplomaResponse>>.Success(result.Result, HttpStatusCode.OK));
    }

    [HttpGet("{id}/quizzes")]
    public async Task<IActionResult> GetDiplomaQuizzes(Guid id, Guid studentId)
    {
        var result = await _mediator.Send(new GetDiplomaQuizezQuery(id, studentId));
        return Ok(ApiResponse<List<GetDiplomaQuizezResponse>>.Success(result.Result, HttpStatusCode.OK));
    }
}