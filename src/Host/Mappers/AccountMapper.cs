using Core.Models;
using Host.Models;

namespace Host.Mappers;

public static class AccountMapper
{
    public static AccountResponse ToResponse(this Account account)
    {
        return new AccountResponse
        {
            Id = account.Id,
            Name = account.Name,
            CreatedAt = account.CreatedAt,
            Credits = account.Credits.Select(c => c.ToResponse()).ToList()
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
