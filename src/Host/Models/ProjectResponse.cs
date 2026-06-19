namespace Host.Models;

public record ProjectResponse
{
    public required Guid Id { init; get; }
    public required string Name { init; get; }
    public required string Country { init; get; }
    public required string Type { init; get; }
}
