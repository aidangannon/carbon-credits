namespace Persistence.Models;

public record FileRecord
{
    public required object Value { get; init; }
    public required MetaRecord Meta { get; init; }
}
