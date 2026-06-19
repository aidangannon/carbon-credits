namespace Host.Models;

public record CreateAccountRequest
{
    public required string Name { init; get; }
}
