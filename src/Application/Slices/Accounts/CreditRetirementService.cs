using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Accounts;

public interface ICreditRetirementService
{
    Task<Result<Credit>> RetireCredit(Guid accountId, Guid creditId, CancellationToken cancellationToken);
}

public class CreditRetirementService(IAccountRepository accountRepository) : ICreditRetirementService
{
    public async Task<Result<Credit>> RetireCredit(Guid accountId, Guid creditId, CancellationToken cancellationToken)
    {
        var accountResult = await accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (accountResult.HasFailed())
        {
            return Result<Credit>.Err(accountResult.Error);
        }

        var account = accountResult.Unwrap();

        var domainResult = account.RetireCredit(creditId);
        if (domainResult.HasFailed())
        {
            return Result<Credit>.Err(domainResult.Error!);
        }

        var saveResult = await accountRepository.SaveAsync(account, cancellationToken);
        if (saveResult.HasFailed())
        {
            return Result<Credit>.Err(saveResult.Error);
        }

        var credit = account.Credits.First(c => c.Id == creditId);
        return Result<Credit>.Ok(credit);
    }
}
