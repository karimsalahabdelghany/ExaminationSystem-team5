namespace ExaminationSystem.API.Results;

public class CustomForbidResult : IActionResult
{
    private readonly string _message;

    public CustomForbidResult(string message = "Access denied")
    {
        _message = message;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.StatusCode = StatusCodes.Status403Forbidden;
        response.ContentType = "application/json";

        var body = ReqestResult<object>.Failure(_message ,HttpStatusCode.Forbidden);
        await response.WriteAsJsonAsync(body);
    }
}
