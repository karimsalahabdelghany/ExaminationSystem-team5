using Microsoft.AspNetCore.Authorization.Policy;



namespace ExaminationSystem.API.Middleware;

public class CustomAuthorizationMiddlewareResultHandler
    : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _default = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var body = ApiResponse<object>.Failure("You are not authorized to perform this action.", HttpStatusCode.Forbidden);
            await context.Response.WriteAsJsonAsync(body);
            return;
        }

        await _default.HandleAsync(next, context, policy, authorizeResult);
    }
}