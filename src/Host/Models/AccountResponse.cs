namespace Host.Models;

public record AccountResponse
{
    public required Guid Id { init; get; }
    public required string Name { init; get; }
    public required DateTime CreatedAt { init; get; }
    public required IReadOnlyCollection<CreditResponse> Credits { init; get; }
}
