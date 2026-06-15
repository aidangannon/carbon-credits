namespace Crosscutting.Result;

public record Result
{
    public required string? Error { get; init; }

    public static Result Ok()
    {
        return new Result
        {
            Error = null
        };
    }

    public static Result Err(string? message)
    {
        return new Result
        {
            Error = message
        };
    }

    public bool HasFailed()
    {
        return Error != null;
    }
}

public record Result<T>
{
    public required string? Error { get; init; }
    public required T? Value { get; init; }

    public static Result<T> Ok(T value)
    {
        return new Result<T>
        {
            Error = null,
            Value = value
        };
    }

    public static Result<T> Err(string? message)
    {
        return new Result<T>
        {
            Error = message,
            Value = default!
        };
    }

    public bool HasFailed()
    {
        return Error != null;
    }

    public T Unwrap()
    {
        if (HasFailed())
        {
            throw new ArgumentNullException(nameof(Value), "Result value is null, failed to unwrap");
        }

        return Value!;
    }
}
