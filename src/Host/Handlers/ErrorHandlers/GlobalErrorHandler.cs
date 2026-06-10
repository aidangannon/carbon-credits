using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Host.Handlers.ErrorHandlers;

public class GlobalErrorHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unhandled exception has occurred please try again later!";

        switch (exception)
        {
            case ValidationException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case OperationCanceledException or TaskCanceledException:
                statusCode = HttpStatusCode.RequestTimeout;
                message = exception.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "User not authorized to perform this operation!";
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
        }

        var problemDetails = new ProblemDetails
        {
            Title = message,
            Status = (int)statusCode,
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? 500;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
