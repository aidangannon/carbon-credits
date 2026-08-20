namespace Persistence.Models;

public record FileRecord<T>
{
    public required T Value { get; init; }
    public required MetaRecord Meta { get; init; }
}
