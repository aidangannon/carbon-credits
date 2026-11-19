using Application.Ports;
using Core.Result;

namespace Application.Slices.Accounts;

public interface IBulkCreditRetirementService
{
    Task<BulkRetireCreditsResult> RetireCredits(Guid accountId, IReadOnlyCollection<Guid> creditIds, CancellationToken cancellationToken);
}

public class BulkCreditRetirementService(IAccountRepository accountRepository) : IBulkCreditRetirementService
{
    public async Task<BulkRetireCreditsResult> RetireCredits(Guid accountId, IReadOnlyCollection<Guid> creditIds, CancellationToken cancellationToken)
    {
        var accountResult = await accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (accountResult.HasFailed())
        {
            return BulkRetireCreditsResult.AccountError(accountResult.Error!);
        }

        var result = accountResult.Unwrap().RetireCredits(creditIds);
        if (result.HasFailed())
        {
            return result;
        }

        var saveResult = await accountRepository.SaveAsync(cancellationToken);
        return saveResult.HasFailed()
            ? BulkRetireCreditsResult.AccountError(saveResult.Error!)
            : result;
    }
}
