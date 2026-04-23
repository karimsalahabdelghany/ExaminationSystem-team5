using ExaminationSystem.API.Results;
using ExaminationSystem.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController(IMediator mediator) : ControllerBase
{
    protected readonly IMediator _mediator = mediator;

    // Forbid() with a message
    protected IActionResult Forbid(string message)
        => new CustomForbidResult(message);

    // Override the parameterless base Forbid() too
    protected new IActionResult Forbid()
        => new CustomForbidResult("You do not have permission to access this resource.");
   
       
    
}
