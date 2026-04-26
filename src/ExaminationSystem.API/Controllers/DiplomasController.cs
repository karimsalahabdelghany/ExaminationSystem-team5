using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Features.Diplomas.CreateDiploma;
using ExaminationSystem.Application.Features.Diplomas.DeleteDiploma;
using ExaminationSystem.Application.Features.Diplomas.GetDiplomas;
using ExaminationSystem.Application.Features.Diplomas.GetDiplomasQuizzes;
using ExaminationSystem.Application.Features.Diplomas.GetPublishedDiplomaQuizez;
using ExaminationSystem.Application.Features.Diplomas.GetStudentDiplomas;
using ExaminationSystem.Application.Features.Diplomas.UpdateDiploma;
using ExaminationSystem.Application.Features.User.GetStudentAttempts;
using ExaminationSystem.Application.Features.User.Orchestrators;
using ExaminationSystem.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ExaminationSystem.API.Controllers;

[Authorize]
public class DiplomasController : BaseController
{
    private readonly ICurrentUser _currentUser;

    public DiplomasController(ICurrentUser currentUser,IMediator mediator) :base(mediator)
    {
        _currentUser = currentUser;
    }
    
    [HttpPost]

    public async Task<IActionResult> Create(CreateDiplomaCommand command , CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command ,cancellationToken);
        if (result is null)
            return BadRequest(ApiResponse<CreateDiplomaResponse>.Failure("Can't Create this diploma!"));
        return Created($"/Diplomas/{result?.Result?.Id}", ApiResponse<CreateDiplomaResponse>.Success(result?.Result,HttpStatusCode.Created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateDiplomaCommand command , CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command , cancellationToken);
        if (result is null)
            return BadRequest(ApiResponse<UpdateDiplomaResult>.Failure("Can't update this diploma!"));
        return Ok(ApiResponse<UpdateDiplomaResult>.Success(result.Result, HttpStatusCode.OK));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id , CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteDiplomaOrchestrator(id) ,cancellationToken);
        return result.Code switch
        {
            ResultCode.DiplomaNotFound => NotFound(ApiResponse<bool>.Failure("Diploma not found", HttpStatusCode.NotFound)),
            ResultCode.DiplomaHasActiveEnrollmentsOrPublished => Conflict(ApiResponse<bool>.Failure("Can't delete this diploma because it has active enrollments or is published", HttpStatusCode.Conflict)),
            _ => Ok(ApiResponse<bool>.Success(true, HttpStatusCode.OK))
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentPublishedDiplomas(
    [FromQuery] int page = 1,
    [FromQuery] int per_page = 20)
    {
        var result = await _mediator.Send(new GetStudentPublishedDiplomasQuery(
            Params: new PaginationParams { Page = page, PerPage = per_page }
        ));

        if (result.Success)
            return Ok(ApiResponse<PaginatedResult<GetStudentPuplishedDiplomasResponse>>
                .Success(
                value: result.Result,
                meta: new
                {
                    page = result.Result.Page,
                    per_page = result.Result.PerPage,
                    total = result.Result.Total,
                    totalPages = result.Result.TotalPages

                },
                HttpStatusCode.OK));

        return result.Code switch
        {
            ResultCode.DiplomaNotFound =>
                NotFound(ApiResponse<PaginatedResult<GetStudentPuplishedDiplomasResponse>>
                    .Failure("No diplomas found.", HttpStatusCode.NotFound)),

            _ => BadRequest(ApiResponse<PaginatedResult<GetStudentPuplishedDiplomasResponse>>
                    .Failure("Could not load diplomas.", HttpStatusCode.BadRequest))
        };
    }

    [HttpGet("{id}/quizzes")]
    public async Task<IActionResult> GetDiplomaQuizzes(
    [FromRoute(Name = "id")] Guid diplomaId,
    [FromQuery] int page = 1,
    [FromQuery] int per_page = 20)
    {
        bool isValidUserid = _currentUser.TryGetUserId(out Guid studentId);
        if (!isValidUserid)
            return Unauthorized(ApiResponse<PaginatedResult<GetPublishedDiplomaQuizezResponse>>
                .Failure("Invalid token claims.", HttpStatusCode.Unauthorized));


        var result = await _mediator.Send(new GetPublishedDiplomaQuizzesQuery(
            DiplomaId: diplomaId,
            Params: new PaginationParams { Page = page, PerPage = per_page }
        ));

        if (result.Success)
            return Ok(ApiResponse<IEnumerable<GetPublishedDiplomaQuizezResponse>>.Success(
                value : result.Result.Data,
                statusCode : HttpStatusCode.OK,
                meta : new
                {
                    page = result.Result.Page,
                    per_page = result.Result.PerPage,
                    total = result.Result.Total,
                    totalPages = result.Result.TotalPages

                }
            ));

        return result.Code switch
        {
            ResultCode.DiplomaNotFound =>
                NotFound(ApiResponse<IEnumerable<GetPublishedDiplomaQuizezResponse>>
                    .Failure("Diploma not found.", HttpStatusCode.NotFound)),

            ResultCode.StudentNotEnrolledInDiploma =>
                StatusCode(403, ApiResponse<IEnumerable<GetPublishedDiplomaQuizezResponse>>
                    .Failure("You are not enrolled in this diploma.",
                        HttpStatusCode.Forbidden)),

            _ => BadRequest(ApiResponse < IEnumerable < GetPublishedDiplomaQuizezResponse >>
                    .Failure("Could not load quizzes.", HttpStatusCode.BadRequest))
        };
    
    }
    // GET /api/diplomas/Diploma? page = 1 & per_page = 20
    [HttpGet("Diploma")]
    public async Task<IActionResult> GetDiplomas(
        [FromQuery] int page = 1,
        [FromQuery] int per_page = 20)
    {
        bool isvaliduser = _currentUser.TryGetUserId(out Guid studentId);
        if (!isvaliduser)
            return Unauthorized(ApiResponse<GetDiplomasResponse>.
                   Failure("Invalid token claims.", HttpStatusCode.Unauthorized));

        var result = await _mediator.Send(new GetDiplomasQuery(
            Params: new PaginationParams { Page = page, PerPage = per_page }
        ));
        if (result.Success)
            return Ok(ApiResponse<PaginatedResult<GetDiplomasResponse>>.Success(
                value : result.Result,
                statusCode: HttpStatusCode.OK,
                meta : new
                { 
                    page = result.Result.Page,
                    per_page = result.Result.PerPage,
                    total = result.Result.Total,
                    totalPages =result.Result.TotalPages
                }
                ));

        return result.Code switch
        {
            ResultCode.DiplomaNotFound =>
                NotFound(ApiResponse<PaginatedResult<GetDiplomasResponse>>
                    .Failure("No diplomas found.", HttpStatusCode.NotFound)),
            _ => BadRequest(ApiResponse<PaginatedResult<GetDiplomasResponse>>
                    .Failure("Could not load diplomas.", HttpStatusCode.BadRequest))
        };
    }
    
}