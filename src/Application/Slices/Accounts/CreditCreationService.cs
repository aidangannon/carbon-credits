using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Accounts;

public interface ICreditCreationService
{
    Task<Result<Account>> CreateCredit(Guid accountId, Guid projectId, Credit credit, CancellationToken cancellationToken);
}

public class CreditCreationService(IAccountRepository accountRepository, IProjectRepository projectRepository) : ICreditCreationService
{
    public async Task<Result<Account>> CreateCredit(Guid accountId, Guid projectId, Credit credit, CancellationToken cancellationToken)
    {
        var accountResult = await accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (accountResult.HasFailed())
            return Result<Account>.Err(accountResult.Error);

        var projectResult = await projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (projectResult.HasFailed())
            return Result<Account>.Err(projectResult.Error);

        var canCreate = accountResult.Unwrap().CanCreate(projectResult.Unwrap(), credit);
        if (canCreate.HasFailed())
            return Result<Account>.Err(canCreate.Error);

        return await accountRepository.AddCreditAsync(accountId, credit, cancellationToken);
    }
}
