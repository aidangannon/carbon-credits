using Core.Models;
using Crosscutting.Result;

namespace Application.Ports;

public interface IAccountRepository
{
    Task<Result<Account>> GetByIdAsync(Guid id);
}
