using Core.Models;
using Host.Models;

namespace Host.Mappers.Accounts;

public static class AccountMapper
{
    public static AccountResponse ToResponse(this Account account, bool includeRetiredCredits = true, bool includeFutureCredits = false)
    {
        var credits = account.Credits.Where(c =>
            (includeRetiredCredits || !c.IsRetired) &&
            (includeFutureCredits || !c.IsIssuedInFuture));

        return new AccountResponse
        {
            Id = account.Id,
            Name = account.Name,
            CreatedAt = account.CreatedAt,
            Credits = credits.Select(c => c.ToResponse()).ToList()
        };
    }

    public static Account ToAccount(this CreateAccountRequest request)
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            Credits = []
        };
    }
}
