using System.Net;

namespace ExaminationSystem.Application.Responses;

public class ReqestResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; } = default;
    public List<string> Errors { get; set; } = new();
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public ReqestResult() { }
    public ReqestResult(bool success, T? value, List<string> errors, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        IsSuccess = success;
        Value = value;
        Errors = errors ?? new List<string>();
        StatusCode = statusCode;
    }

    // Success response
    public static ReqestResult<T> Success(T? value, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new ReqestResult<T>(true, value, new List<string>(), statusCode);

    // Failure with single error message
    public static ReqestResult<T> Failure(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new ReqestResult<T>(false, default, new List<string> { errorMessage }, statusCode);

    // Failure with multiple error messages
    public static ReqestResult<T> Failure(List<string> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new ReqestResult<T>(false, default, errors ?? new List<string>(), statusCode);

}
