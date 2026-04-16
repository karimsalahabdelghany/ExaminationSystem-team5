namespace ExaminationSystem.Application.Common.Results;

public sealed class Error
{
    public Error(
        string code,
        string message,
        int statusCode,
        Dictionary<string, object?>? metadata = null)
    {
        Code = code;
        Message = message;
        StatusCode = statusCode;
        Metadata = metadata;
    }

    public string Code { get; set; }

    public string Message { get; set; }

    public int StatusCode { get; set; }

    public Dictionary<string, object?>? Metadata { get; set; }
}
