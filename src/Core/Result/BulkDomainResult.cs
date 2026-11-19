namespace Core.Result;

public record CreditFailure(Guid CreditId, string Error);

public record BulkDomainResult
{
    public string? Error { get; init; }
    public IReadOnlyCollection<CreditFailure> Failures { get; init; } = [];

    public static BulkDomainResult Ok() => new();
    public static BulkDomainResult Err(string error) => new() { Error = error };
    public static BulkDomainResult Err(IReadOnlyCollection<CreditFailure> failures) => new() { Failures = failures };
    public bool HasFailed() => Error != null || Failures.Count > 0;
}
