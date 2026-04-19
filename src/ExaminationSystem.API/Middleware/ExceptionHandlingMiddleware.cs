using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Application.Responses;
using Newtonsoft.Json;
using System.Net;

namespace ExaminationSystem.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException vex)
        {
            _logger.LogError(vex, "A validation error occurred: {Message}", vex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var response = JsonConvert.SerializeObject(vex.Failure); ;
            await context.Response.WriteAsync(response);
        }

        catch (ForbiddenException fex)
        {
            _logger.LogWarning(fex, "Forbidden: {Message}", fex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            var apiResponse = ApiResponse<object>
                .Failure(fex.Message, HttpStatusCode.Forbidden);
            var response = JsonConvert.SerializeObject(apiResponse);
            await context.Response.WriteAsync(response);
        }

        catch (NotFoundException nex)
        {
            _logger.LogWarning(nex, "Resource not found: {Message}", nex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";
            var apiResponse = ApiResponse<object>
                .Failure(nex.Message, HttpStatusCode.NotFound);
            var response = JsonConvert.SerializeObject(apiResponse);
            await context.Response.WriteAsync(response);
        }

        catch (ConflictException cex)
        {
            _logger.LogWarning(cex, "Conflict: {Message}", cex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.Response.ContentType = "application/json";
            var apiResponse = ApiResponse<object>
                .Failure(cex.Message, HttpStatusCode.Conflict);
            var response = JsonConvert.SerializeObject(apiResponse);
            await context.Response.WriteAsync(response);
        }

        catch (GoneException gex)
        {
            _logger.LogWarning(gex, "Gone: {Message}", gex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Gone;
            context.Response.ContentType = "application/json";
            var apiResponse = ApiResponse<object>
                .Failure(gex.Message, HttpStatusCode.Gone);
            var response = JsonConvert.SerializeObject(apiResponse);
            await context.Response.WriteAsync(response);
        }

        catch (UnprocessableException uex)
        {
            _logger.LogWarning(uex, "Unprocessable: {Message}", uex.Message);
            context.Response.StatusCode = 422;
            context.Response.ContentType = "application/json";
            var apiResponse = ApiResponse<object>
                .Failure(uex.Message, (HttpStatusCode)422);
            var response = JsonConvert.SerializeObject(apiResponse);
            await context.Response.WriteAsync(response);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            if (_environment.IsDevelopment())
            {
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(ex.Message, HttpStatusCode.InternalServerError));
                return;
            }
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var apiResponse = ApiResponse<object>.Failure("Somthing went wrong!");
            var response = JsonConvert.SerializeObject(apiResponse);
            await context.Response.WriteAsync(response);
        }
    }
}