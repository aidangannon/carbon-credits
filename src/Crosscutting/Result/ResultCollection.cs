namespace Crosscutting.Result;

public record ResultCollection
{
    public required IReadOnlyCollection<Result> Results { get; init; }

    public string? Error => HasFailed()
        ? string.Join(", ", Results.Where(r => r.HasFailed()).Select(r => r.Error))
        : null;

    public static ResultCollection Empty()
    {
        return new ResultCollection
        {
            Results = []
        };
    }

    public static ResultCollection Of(params Result[] results)
    {
        return new ResultCollection
        {
            Results = results
        };
    }

    public ResultCollection Add(Result result)
    {
        return new ResultCollection
        {
            Results = [.. Results, result]
        };
    }

    public bool HasFailed()
    {
        return Results.Any(r => r.HasFailed());
    }

    public Result ToResult()
    {
        return HasFailed()
            ? Result.Err(Error)
            : Result.Ok();
    }
}
