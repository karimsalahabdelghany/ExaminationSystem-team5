using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.API.Abstractions;

public static class ResultExtension
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert successful result to problem details.");
        }

        var error = result.Error!;
        var statusCode = error.StatusCode <= 0 ? StatusCodes.Status400BadRequest : error.StatusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = error.Code,
            Detail = error.Message
        };

        problemDetails.Extensions["errors"] = new[] { error.Code, error.Message };
        if (error.Metadata is not null)
        {
            foreach (var item in error.Metadata)
            {
                problemDetails.Extensions[item.Key] = item.Value;
            }
        }

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
}
