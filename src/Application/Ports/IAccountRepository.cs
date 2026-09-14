using Core.Models;
using Crosscutting.Result;

namespace Application.Ports;

public interface IAccountRepository
{
    Task<Result<Account>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(Account account);
    Task<Result> SaveAsync(CancellationToken cancellationToken);
}
