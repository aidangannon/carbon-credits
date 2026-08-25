namespace Core.Models;

public record Credit
{
    public required Guid Id { get; init; }
    public required DateTime? RetiredAt { get; set; }
    public required DateTime IssuedAt { get; init; }
    public required Guid ProjectId { get; init; }

    public bool IsRetired => RetiredAt is not null;
    public bool IsIssuedInFuture => IssuedAt > DateTime.UtcNow;
}
