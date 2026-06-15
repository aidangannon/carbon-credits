using Crosscutting.Result;
using Host.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Host.Extensions;

public static class ResultExtensions
{
    public static ProblemHttpResult ToProblemResult(this Result result)
    {
        var error = ErrorCodeMapper.ToErrorDetails(result.Error!);
        return TypedResults.Problem(
            statusCode: error.StatusCode,
            title: error.Title,
            detail: result.Error
        );
    }

    public static ProblemHttpResult ToProblemResult<T>(this Result<T> result) where T : class
    {
        var error = ErrorCodeMapper.ToErrorDetails(result.Error!);
        return TypedResults.Problem(
            statusCode: error.StatusCode,
            title: error.Title,
            detail: result.Error
        );
    }
}
