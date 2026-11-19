using Application.Ports;
using Core.Result;

namespace Application.Slices.Accounts;

public interface IBulkCreditRetirementService
{
    Task<BulkDomainResult> RetireCredits(Guid accountId, IReadOnlyCollection<Guid> creditIds, CancellationToken cancellationToken);
}

public class BulkCreditRetirementService(IAccountRepository accountRepository) : IBulkCreditRetirementService
{
    public async Task<BulkDomainResult> RetireCredits(Guid accountId, IReadOnlyCollection<Guid> creditIds, CancellationToken cancellationToken)
    {
        var accountResult = await accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (accountResult.HasFailed())
        {
            return BulkDomainResult.Err(accountResult.Error!);
        }

        var result = accountResult.Unwrap().RetireCredits(creditIds);
        if (result.HasFailed())
        {
            return result;
        }

        var saveResult = await accountRepository.SaveAsync(cancellationToken);
        return saveResult.HasFailed() ? BulkDomainResult.Err(saveResult.Error!) : result;
    }
}
