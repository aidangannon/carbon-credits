using Core.Errors;

namespace Host.Mappers;

public record ErrorDetails(int StatusCode, string Title);

public static class ErrorCodeMapper
{
    public static ErrorDetails ToErrorDetails(string errorCode) => errorCode switch
    {
        AccountErrors.NotFound => new ErrorDetails(StatusCodes.Status404NotFound, "Not found"),
        _ => throw new InvalidOperationException($"Unhandled error code: {errorCode}")
    };
}
