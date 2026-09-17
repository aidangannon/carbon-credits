namespace Host.Models;

public record GetAccountCreditsQuery
{
    public bool? IncludeRetiredCredits { get; init; }
    public bool? IncludeFutureCredits { get; init; }
}
