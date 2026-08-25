using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Accounts;

public interface IAccountRetrievalService
{
    Task<Result<Account>> GetAccountById(Guid id, bool includeRetiredCredits, bool includeFutureCredits, CancellationToken cancellationToken);
}

public class AccountRetrievalService(IAccountRepository accountRepository) : IAccountRetrievalService
{
    public async Task<Result<Account>> GetAccountById(Guid id, bool includeRetiredCredits, bool includeFutureCredits, CancellationToken cancellationToken)
    {
        var result = await accountRepository.GetByIdAsync(id, cancellationToken);

        if (result.HasFailed())
        {
            return result;
        }

        var account = result.Unwrap();

        return Result<Account>.Ok(new Account
        {
            Id = account.Id,
            Name = account.Name,
            CreatedAt = account.CreatedAt,
            Credits = account.GetCredits(includeRetiredCredits, includeFutureCredits)
        });
    }
}
