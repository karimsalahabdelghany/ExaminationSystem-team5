using ExaminationSystem.Application.Features.User.Get_Dashboard.Queries.Caching;
using ExaminationSystem.Application.Features.User.Orchestrators;
using ExaminationSystem.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExaminationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentsController(IMediator mediator)
            => _mediator = mediator;


        
        /// 200 → { success, data: { enrolled_diplomas[], recent_quiz_attempts[], overall_stats } }
        /// 401 → JWT missing or invalid
        /// 403 → valid JWT but role != student
        ///// GET /api/students/dashboard
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<GetStudentDashboardResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDashboard()
        {
            // JWT payload must contain: { user_id, role, exp }
            var studentIdClaim = User.FindFirstValue("user_id");

            if (studentIdClaim is null || !Guid.TryParse(studentIdClaim, out var studentId))
                return Unauthorized(ApiResponse<GetStudentDashboardResponse>.Failure("Invalid token claims."));

            var result = await _mediator.Send(new GetStudentDashboardQuery(studentId));

            if (result is null)
                return BadRequest(ApiResponse<GetStudentDashboardResponse>.Failure(
                    "Could not load dashboard."));


            return Ok(result);
        }
    }
}
