using Core.Models;
using Core.Result;
using Crosscutting.Result;

namespace Application.Ports;

public interface IAccountRepository
{
    Task<Result<Account>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> SaveAsync(Account account, CancellationToken cancellationToken);
    Task<Result<Account>> AddCreditAsync(Guid accountId, Project project, Credit credit, CancellationToken cancellationToken);
}
