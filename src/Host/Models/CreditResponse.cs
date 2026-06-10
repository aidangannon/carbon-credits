namespace Host.Models;

public record CreditResponse
{
    public required Guid Id { get; init; }
    public required DateTime? RetiredAt { get; init; }
    public required DateTime IssuedAt { get; init; }
    public required Guid ProjectId { get; init; }
}
