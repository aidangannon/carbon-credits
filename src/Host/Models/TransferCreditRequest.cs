namespace Host.Models;

public record TransferCreditRequest
{
    public required Guid RecipientAccountId { get; init; }
}
