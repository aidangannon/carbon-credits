using Core.Models;
using Crosscutting.Result;

namespace Application.Slces;

public interface IAccountCreationService
{
    Task<Result<Account>> CreateAccount(Account account);
}
