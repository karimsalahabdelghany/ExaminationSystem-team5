using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Features.Admin.Commands.SetUserLockState;
using ExaminationSystem.Application.Features.Admin.Queries;

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

        // GET /api/admin/analytics?from=&to=&diploma_id=
        [HttpGet("analytics")]
        [ProducesResponseType(typeof(ApiResponse<GetAdminAnalyticsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAnalytics(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery(Name = "diploma_id")] Guid? diplomaId = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetAdminAnalyticsQuery(from, to, diplomaId),
                cancellationToken);

            if (result.Success)
                return Ok(ApiResponse<GetAdminAnalyticsResponse>.Success(result.Result, HttpStatusCode.OK));

            return BadRequest(ApiResponse<GetAdminAnalyticsResponse>.Failure("Invalid analytics filters.", HttpStatusCode.BadRequest));
        }
        // GET /api/admin/attempts?page=1&per_page=20&quiz_id=&student_id=&sort_by=submitted_at&order=desc
        [HttpGet("attempts")]
        public async Task<IActionResult> GetAttempts(
            [FromQuery] int page = 1,
            [FromQuery] int per_page = 20,
            [FromQuery] Guid? quiz_id = null,    // optional filter
            [FromQuery] Guid? student_id = null, // optional filter
            [FromQuery] string? sort_by = null,  // submitted_at | score | status
            [FromQuery] string? order = null)    // asc | desc
        {
            var result = await _mediator.Send(new GetAdminAttemptsQuery(
                Pagination: new PaginationParams { Page = page, PerPage = per_page },
                QuizId: quiz_id,
                StudentId: student_id,
                SortBy: sort_by,
                Order: order
            ));
            if (result.Success)
            {
                return Ok(ApiResponse<PaginatedResult<GetAdminAttemptsResponse>>.Success(
                    value:result.Result
                    ,meta : new
                    {
                        page = result.Result.Page,
                        perPage = result.Result.PerPage,
                        total = result.Result.Total,
                        totalPages = result.Result.TotalPages
                    }
                    , HttpStatusCode.OK));
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

        // GET /api/admin/attempts/{attemptId}
        [HttpGet("attempts/{attemptId:guid}")]
        public async Task<IActionResult> GetAttemptDetails(Guid attemptId)
        {
            var result = await _mediator.Send(new GetAdminAttemptDetailsQuery(attemptId));

            return result.Code switch
            {
                ResultCode.AttemptNotFound =>
                    NotFound(ApiResponse<GetAdminAttemptDetailsResponse>
                        .Failure("Attempt not found.", HttpStatusCode.NotFound)),

                _ => Ok(ApiResponse<GetAdminAttemptDetailsResponse>.Success(
                    value :result.Result, HttpStatusCode.OK))
                
            };
        }

        [HttpPost("users/{userId:guid}/lock")]
        public async Task<IActionResult> LockUser(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetUserLockStateCommand(userId, true), cancellationToken);
            return result.Code switch
            {
                ResultCode.AccountLockedByAdmin =>
                    Ok(ApiResponse<string>.Success("User account locked successfully.", HttpStatusCode.OK)),

                ResultCode.UserIsNotExsit =>
                    NotFound(ApiResponse<string>.Failure("User not found.", HttpStatusCode.NotFound)),

                _ =>
                    BadRequest(ApiResponse<string>.Failure("Could not lock user account.", HttpStatusCode.BadRequest))
            };
        }

        [HttpPost("users/{userId:guid}/unlock")]
        public async Task<IActionResult> UnlockUser(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetUserLockStateCommand(userId, false), cancellationToken);
            return result.Code switch
            {
                ResultCode.AccountUnlockedByAdmin =>
                    Ok(ApiResponse<string>.Success("User account unlocked successfully.", HttpStatusCode.OK)),

                ResultCode.UserIsNotExsit =>
                    NotFound(ApiResponse<string>.Failure("User not found.", HttpStatusCode.NotFound)),

                _ =>
                    BadRequest(ApiResponse<string>.Failure("Could not unlock user account.", HttpStatusCode.BadRequest))
            };
        }

        [HttpGet("users/{id:guid}/status")]
        public async Task<IActionResult> GetUserStatus(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAdminUserStatusQuery(id), cancellationToken);

            return result.Code switch
            {
                ResultCode.AdminUserStatusRetrievedSuccessfully =>
                    Ok(ApiResponse<GetAdminUserStatusResponse>.Success(result.Result, HttpStatusCode.OK)),

                ResultCode.UserIsNotExsit =>
                    NotFound(ApiResponse<GetAdminUserStatusResponse>.Failure("User not found.", HttpStatusCode.NotFound)),

                _ =>
                    BadRequest(ApiResponse<GetAdminUserStatusResponse>.Failure("Could not get user status.", HttpStatusCode.BadRequest))
            };
        }

    }
}
