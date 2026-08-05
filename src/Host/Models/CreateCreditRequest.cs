namespace Host.Models;

public record CreateCreditRequest
{
    public required DateTime IssuedAt { get; init; }
    public required Guid ProjectId { get; init; }
}
