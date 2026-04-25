using System.Net;

namespace ExaminationSystem.Application.Responses;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; } = default;
    public List<string> Errors { get; set; } = new();
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public object? Meta { get; set; } = null;
    public ApiResponse() { }
    public ApiResponse(bool success, T? value, List<string> errors, HttpStatusCode statusCode = HttpStatusCode.OK ,Object? meta =null)
    {
        IsSuccess = success;
        Value = value;
        Errors = errors ?? new List<string>();
        StatusCode = statusCode;
        Meta = meta;
    }

    // Success response without meta (non-paginated endpoints)
    public static ApiResponse<T> Success(T? value, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new ApiResponse<T>(
            success:    true,
            value:      value,
            errors:     new List<string>(),
            statusCode: statusCode,
            meta:       null);
    //Success with meta (paginated endpoints)
    public static ApiResponse<T> Success(T? value, Object? meta, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new ApiResponse<T>(
            success:    true,
            value:      value,
            errors:     new List<string>(),
            statusCode: statusCode,
            meta:       meta);

    // Failure with single error message
    public static ApiResponse<T> Failure(
        string errorMessage, 
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new ApiResponse<T>(false, default, new List<string> { errorMessage }, statusCode,meta : null);

    // Failure with multiple error messages
    public static ApiResponse<T> Failure(List<string> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new ApiResponse<T>(false, default, errors ?? new List<string>(), statusCode,meta : null);

}
