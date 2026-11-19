namespace Core.Result;

public record CreditRetirementFailure(Guid CreditId, string Error);

public record BulkRetireCreditsResult
{
    public string? Error { get; init; }
    public IReadOnlyCollection<CreditRetirementFailure> Failures { get; init; } = [];

    public static BulkRetireCreditsResult Ok() => new();
    public static BulkRetireCreditsResult AccountError(string error) => new() { Error = error };
    public static BulkRetireCreditsResult CreditFailures(IReadOnlyCollection<CreditRetirementFailure> failures) =>
        new() { Error = Errors.CreditErrors.BulkRetireFailed, Failures = failures };

    public bool HasFailed() => Error != null;
}
