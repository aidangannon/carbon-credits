namespace Host.Models;

public record CreateProjectRequest
{
    public required string Name { init; get; }
    public required string Country { init; get; }
    public required string Type { init; get; }
}
