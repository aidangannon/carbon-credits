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
        {
            return Result<Account>.Err(accountResult.Error);
        }

        var account = accountResult.Unwrap();

        var projectResult = await projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (projectResult.HasFailed())
        {
            return Result<Account>.Err(projectResult.Error);
        }

        var project = projectResult.Unwrap();

        var domainResult = account.AddCredit(project, credit);
        if (domainResult.HasFailed())
        {
            return Result<Account>.Err(domainResult.Error!);
        }

        var saveResult = await accountRepository.SaveAsync(account, cancellationToken);

        return saveResult.HasFailed() ? Result<Account>.Err(saveResult.Error) : Result<Account>.Ok(account);
    }
}
