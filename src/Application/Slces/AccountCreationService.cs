using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slces;

public class AccountCreationService(IAccountRepository accountRepository) : IAccountCreationService
{
    public async Task<Result<Account>> CreateAccount(Account account)
    {
        var result = await accountRepository.SaveAsync(account);

        if (result.HasFailed())
        {
            return Result<Account>.Err(result.Error);
        }

        return Result<Account>.Ok(account);
    }
}
