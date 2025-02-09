namespace Parsalud.BusinessLayer.Abstractions;

public class BusinessResponse
{
    public required bool IsSuccess { get; init; }
    public required string? Message { get; init; }

    public static BusinessResponse Success()
    {
        return new BusinessResponse
        {
            IsSuccess = true,
            Message = null,
        };
    }

    public static BusinessResponse<T> Success<T>(T? data)
        where T : class
    {
        return new BusinessResponse<T>
        {
            IsSuccess = true,
            Message = null,
            Data = data,
        };
    }

    public static BusinessResponse Error(string message)
    {
        return new BusinessResponse
        {
            IsSuccess = false,
            Message = message,
        };
    }

    public static BusinessResponse<T> Error<T>(string message)
        where T : class
    {
        return new BusinessResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Data = null,
        };
    }
}

public class BusinessResponse<T> : BusinessResponse
    where T : class
{
    public required T? Data { get; init; }
}
