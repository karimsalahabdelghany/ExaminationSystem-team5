namespace ExaminationSystem.Application.Common.Services.EmailService;

public class EmailResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    private EmailResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static EmailResult Success() => new(true);
    public static EmailResult Fail(string error) => new(false, error);
}