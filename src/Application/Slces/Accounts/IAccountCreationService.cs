using Core.Models;
using Crosscutting.Result;

namespace Application.Slces.Accounts;

public interface IAccountCreationService
{
    Task<Result<Account>> CreateAccount(Account account, CancellationToken cancellationToken);
}
