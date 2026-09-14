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
        accountRepository.Add(account);
        var result = await accountRepository.SaveAsync(cancellationToken);

        return result.HasFailed() ? Result<Account>.Err(result.Error) : Result<Account>.Ok(account);
    }
}
