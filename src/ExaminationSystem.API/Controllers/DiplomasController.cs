using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Features.Diplomas.CreateDiploma;
using ExaminationSystem.Application.Features.Diplomas.DeleteDiploma;
using ExaminationSystem.Application.Features.Diplomas.GetPublishedDiplomaQuizez;
using ExaminationSystem.Application.Features.Diplomas.GetStudentDiplomas;
using ExaminationSystem.Application.Features.Diplomas.UpdateDiploma;

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
        return result.Code switch
        {
            ResultCode.DiplomaNotFound => NotFound(ApiResponse<bool>.Failure("Diploma not found", HttpStatusCode.NotFound)),
            ResultCode.DiplomaHasActiveEnrollmentsOrPublished => Conflict(ApiResponse<bool>.Failure("Can't delete this diploma because it has active enrollments or is published", HttpStatusCode.Conflict)),
            _ => Ok(ApiResponse<bool>.Success(true, HttpStatusCode.OK))
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentPublishedDiplomas([FromQuery] GetStudentPuplishedDiplomasQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<PaginationResult<GetStudentPuplishedDiplomasResponse>>.Success(result.Result, HttpStatusCode.OK));
    }

    [HttpGet("{id}/quizzes")]
    public async Task<IActionResult> GetDiplomaQuizzes(Guid id, Guid studentId)
    {
        var result = await _mediator.Send(new GetPublishedDiplomaQuizezQuery(id, studentId));
        return result.Code switch
        {
            ResultCode.DiplomaNotFound => NotFound(ApiResponse<List<GetPublishedDiplomaQuizezResponse>>.Failure("Diploma not found", HttpStatusCode.NotFound)),
            ResultCode.StudentNotEnrolledInDiploma => Forbid("Student not enrolled in diploma"),
            _ => Ok(ApiResponse<List<GetPublishedDiplomaQuizezResponse>>.Success(result.Result, HttpStatusCode.OK))
        };
    }
}