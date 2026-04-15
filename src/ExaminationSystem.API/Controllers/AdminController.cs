using ExaminationSystem.Application.Features.Admin.Queries;
using ExaminationSystem.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
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
    }
}
