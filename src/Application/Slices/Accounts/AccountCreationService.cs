using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Accounts;

public interface IAccountCreationService
{
    Task<Result<Account>> CreateAccount(Account account, CancellationToken cancellationToken);
}

public class AccountCreationService(IAccountRepository accountRepository) : IAccountCreationService
{
    public async Task<Result<Account>> CreateAccount(Account account, CancellationToken cancellationToken)
    {
        var result = await accountRepository.SaveAsync(account, cancellationToken);

        if (result.HasFailed())
        {
            return Result<Account>.Err(result.Error);
        }

        return Result<Account>.Ok(account);
    }
}
