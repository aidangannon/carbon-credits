namespace Core.Models;

public record Project
{
    public required Guid Id { set; get; }
    public required string Name { set; get; }
    public required string Country { set; get; }
    public required ProjectType Type { set; get; }
}
