using System.Diagnostics.CodeAnalysis;

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

public class BusinessResponse<T>
    where T : class
{
    public required string? Message { get; init; }

    /// <summary>
    /// The request was successful.
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// The request was successful and the data is not null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Data))]
    public bool IsSuccessWithData { get => IsSuccess && Data is not null; }

    public required T? Data { get; init; }

    public static implicit operator BusinessResponse(BusinessResponse<T> instance)
    {
        return new BusinessResponse
        {
            IsSuccess = instance.IsSuccess,
            Message = instance.Message,
        };
    }
}
