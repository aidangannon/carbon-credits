using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Accounts;

public interface IAccountRetrievalService
{
    Task<Result<Account>> GetAccountById(Guid id, CancellationToken cancellationToken);
}

public class AccountRetrievalService(IAccountRepository accountRepository) : IAccountRetrievalService
{
    public async Task<Result<Account>> GetAccountById(Guid id, CancellationToken cancellationToken)
    {
        return await accountRepository.GetByIdAsync(id, cancellationToken);
    }
}
