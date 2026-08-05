namespace Core.Result;

public record DomainResult
{
    public string? Error { get; init; }

    public static DomainResult Ok() => new() { Error = null };
    public static DomainResult Err(string error) => new() { Error = error };
    public bool HasFailed() => Error != null;
}
