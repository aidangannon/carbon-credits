namespace Host.Models;

public record BulkRetireCreditsRequest
{
    public required IReadOnlyCollection<Guid> CreditIds { get; init; }
}
