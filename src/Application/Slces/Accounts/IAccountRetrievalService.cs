using Core.Models;
using Crosscutting.Result;

namespace Application.Slces.Accounts;

public interface IAccountRetrievalService
{
    Task<Result<Account>> GetAccountById(Guid id, CancellationToken cancellationToken);
}
