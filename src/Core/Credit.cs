namespace Core;

public record Credit
{
    public required Guid Id { get; set; }
    public required DateTime? RetiredAt { get; set; }
    public required DateTime IssuedAt { get; set; }
    public required Guid ProjectId { get; set; }
}
