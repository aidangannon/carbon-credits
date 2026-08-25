using Application.Ports;
using Crosscutting.Result;

namespace Application.Slices.Accounts;

public interface ICreditRetirementService
{
    Task<Result> RetireCredit(Guid accountId, Guid creditId, CancellationToken cancellationToken);
}

public class CreditRetirementService(IAccountRepository accountRepository) : ICreditRetirementService
{
    public async Task<Result> RetireCredit(Guid accountId, Guid creditId, CancellationToken cancellationToken)
    {
        var accountResult = await accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (accountResult.HasFailed())
        {
            return Result.Err(accountResult.Error);
        }

        var account = accountResult.Unwrap();

        var domainResult = account.RetireCredit(creditId);
        if (domainResult.HasFailed())
        {
            return Result.Err(domainResult.Error!);
        }

        return await accountRepository.SaveAsync(account, cancellationToken);
    }
}
