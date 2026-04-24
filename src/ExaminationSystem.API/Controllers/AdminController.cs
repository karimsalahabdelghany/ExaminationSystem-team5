using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Features.Admin.Queries;
using ExaminationSystem.Application.Features.Diplomas.GetDiplomas;
using ExaminationSystem.Application.Responses;
using ExaminationSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }
        /// GET /api/admin/stats
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ApiResponse<GetAdminStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // no login 
        [ProducesResponseType(StatusCodes.Status403Forbidden)]   //unauthrized user is not admin
        public async Task<IActionResult> GetStats()
        {
            var result = await _mediator.Send(new GetAdminStatsQuery());

            if (result is null)
                return BadRequest(
                    ApiResponse<GetAdminStatsResponse>.Failure("Can't retrieve stats!"));

            return Ok(result);
        }
        // GET /api/admin/attempts?page=1&per_page=20&quiz_id=&student_id=
        [HttpGet("attempts")]
        public async Task<IActionResult> GetAttempts(
            [FromQuery] int page = 1,
            [FromQuery] int per_page = 20,
            [FromQuery] Guid? quiz_id = null,   // optional filter
            [FromQuery] Guid? student_id = null)   // optional filter
        {
            var result = await _mediator.Send(new GetAdminAttemptsQuery(
                Pagination: new PaginationParams { Page = page, PerPage = per_page },
                QuizId: quiz_id,
                StudentId: student_id
            ));
            if(result.Success)
            {
                return Ok(ApiResponse<PaginatedResult<GetAdminAttemptsResponse>>.Success(result.Result, HttpStatusCode.OK));
            }

            return result.Code switch
            {
                ResultCode.AttemptNotFound =>
                    NotFound(ApiResponse<PaginatedResult<GetAdminAttemptsResponse>>
                        .Failure("No attempts found.", HttpStatusCode.NotFound)),

                _ => BadRequest(ApiResponse<PaginatedResult<GetAdminAttemptsResponse>>
                        .Failure("Could not load attempts.", HttpStatusCode.BadRequest))
            };
        }
     
    }
}
