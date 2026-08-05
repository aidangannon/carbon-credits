using Core.Errors;

namespace Host.Mappers;

public record ErrorDetails(int StatusCode, string Title);

public static class ErrorCodeMapper
{
    public static ErrorDetails ToErrorDetails(string errorCode) => errorCode switch
    {
        AccountErrors.NotFound => new ErrorDetails(StatusCodes.Status404NotFound, "Not found"),
        ProjectErrors.NotFound => new ErrorDetails(StatusCodes.Status404NotFound, "Not found"),
        CreditErrors.CannotCreateRetired => new ErrorDetails(StatusCodes.Status422UnprocessableEntity, "Unprocessable entity"),
        CreditErrors.IssuedInFuture => new ErrorDetails(StatusCodes.Status422UnprocessableEntity, "Unprocessable entity"),
        CreditErrors.ProjectMismatch => new ErrorDetails(StatusCodes.Status422UnprocessableEntity, "Unprocessable entity"),
        _ => throw new InvalidOperationException($"Unhandled error code: {errorCode}")
    };
}
