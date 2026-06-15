using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slces;

public class AccountRetrievalService(IAccountRepository accountRepository) : IAccountRetrievalService
{
    public async Task<Result<Account>> GetAccountById(Guid id)
    {
        return await accountRepository.GetByIdAsync(id);
    }
}
