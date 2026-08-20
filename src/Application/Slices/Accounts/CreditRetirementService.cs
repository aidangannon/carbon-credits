using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Accounts;

public interface ICreditRetirementService
{
    Task<Result<Account>> RetireCredit(Guid accountId, Guid creditId, CancellationToken cancellationToken);
}

public class CreditRetirementService(IAccountRepository accountRepository) : ICreditRetirementService
{
    public async Task<Result<Account>> RetireCredit(Guid accountId, Guid creditId, CancellationToken cancellationToken)
    {
        var accountResult = await accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (accountResult.HasFailed())
        {
            return Result<Account>.Err(accountResult.Error);
        }

        var account = accountResult.Unwrap();

        var retireResult = account.RetireCredit(creditId);
        if (retireResult.HasFailed())
        {
            return Result<Account>.Err(retireResult.Error);
        }

        var saveResult = await accountRepository.SaveAsync(account, cancellationToken);

        return saveResult.HasFailed() ? Result<Account>.Err(saveResult.Error) : Result<Account>.Ok(account);
    }
}
