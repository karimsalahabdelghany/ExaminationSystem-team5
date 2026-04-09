namespace ExaminationSystem.Application.Common.Results;

public class RequestResult<TResult>
{
    public TResult? Result { get; set; }
    public ResultCode Code { get; set; }
    public bool Success { get; set; }

    public static RequestResult<TResult> Failure(TResult result, ResultCode code)
    {
        return new RequestResult<TResult>
        {
            Code = code,
            Success = false,
            Result = result
        };
    }

    public static RequestResult<object> Failure(ResultCode code)
    {
        return new RequestResult<object>
        {
            Code = code,
            Success = false,
            Result = null
        };
    }

    public static RequestResult<TResult> succeeded(TResult result, ResultCode code)
    {
        return new RequestResult<TResult>
        {
            Code = code,
            Success = true,
            Result = result
        };
    }
}

