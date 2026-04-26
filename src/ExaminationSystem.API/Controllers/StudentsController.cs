using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Features.Attempts.SubmitAttempt;
using ExaminationSystem.Application.Features.User.Get_Dashboard.Queries.Caching;
using ExaminationSystem.Application.Features.User.GetStudentAttempts;
using ExaminationSystem.Application.Features.User.Orchestrators;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExaminationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentsController : BaseController
    {
        private readonly ICurrentUser _currentUser;

        public StudentsController(IMediator mediator, ICurrentUser currentUser)
            : base(mediator)
        {
           _currentUser = currentUser;
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<GetStudentDashboardResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDashboard()
        {
            bool isvaliduser = _currentUser.TryGetUserId(out Guid studentId);

            if (!isvaliduser)
                return Unauthorized(
                   ApiResponse<GetStudentDashboardResponse>.
                   Failure("Invalid token claims.", HttpStatusCode.Unauthorized));

            var result = await _mediator.Send(new GetStudentDashboardQuery());

            return result.Code switch
            {
                ResultCode.StudentsDashoardQuerySucessfull =>        
                Ok(ApiResponse<GetStudentDashboardResponse>
                     .Success(result.Result, HttpStatusCode.OK)),

                ResultCode.StudentStatsDataAlreadyCachedinMemory =>
                    Ok(ApiResponse<GetStudentDashboardResponse>
                        .Success(result.Result, HttpStatusCode.OK)),

                ResultCode.StudentsDashoardQueryFalied =>
                    BadRequest(ApiResponse<GetStudentDashboardResponse>
                        .Failure("Could not load student dashboard.", HttpStatusCode.BadRequest)),

                _ => BadRequest(ApiResponse<GetStudentDashboardResponse>
                        .Failure("Unexpected error.", HttpStatusCode.BadRequest))
            };
        
        }
        [HttpGet("attempts")]
        public async Task<IActionResult> GetAttempts(
       [FromQuery] int page = 1,
       [FromQuery] int per_page = 20,
       [FromQuery] Guid? quiz_id = null,
       [FromQuery] Guid? diploma_id = null)
        {
            //checks if Id claim is valid Guid
            bool isvaliduser = _currentUser.TryGetUserId(out Guid studentId);
            if (!isvaliduser) return Unauthorized(ApiResponse<PaginatedResult<GetStudentAttemptsResponse>>.
               Failure("Invalid token claims.", HttpStatusCode.Unauthorized));

            var result = await _mediator.Send(new GetStudentAttemptsQuery(
                Pagination: new PaginationParams { Page = page, PerPage = per_page },
                QuizId: quiz_id,
                DiplomaId: diploma_id
            ));
            if (result.Success)
                return Ok(ApiResponse<PaginatedResult<GetStudentAttemptsResponse>>
                    .Success(
                    value: result.Result,
                    meta: new
                    {
                        page = result.Result.Page,
                        per_page = result.Result.PerPage,
                        total= result.Result.Total,
                        totalpages = result.Result.TotalPages
                    },
                    HttpStatusCode.OK));

            return result.Code switch
            {
                ResultCode.RecentQuizAttemptsloadedSuccessfuly =>
                    Ok(ApiResponse<PaginatedResult<GetStudentAttemptsResponse>>
                        .Success(result.Result, HttpStatusCode.OK)),

                _ => BadRequest(ApiResponse<PaginatedResult<GetStudentAttemptsResponse>>
                        .Failure("Could not load attempts.", HttpStatusCode.BadRequest))
            };


        }     

        
    }
}

