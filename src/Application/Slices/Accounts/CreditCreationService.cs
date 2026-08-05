using Application.Ports;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Accounts;

public interface ICreditCreationService
{
    Task<Result<Account>> CreateCredit(Guid accountId, Credit credit, CancellationToken cancellationToken);
}

public class CreditCreationService(IAccountRepository accountRepository, IProjectRepository projectRepository) : ICreditCreationService
{
    public async Task<Result<Account>> CreateCredit(Guid accountId, Credit credit, CancellationToken cancellationToken)
    {
        var projectResult = await projectRepository.GetByIdAsync(credit.ProjectId, cancellationToken);
        if (projectResult.HasFailed())
            return Result<Account>.Err(projectResult.Error);

        var project = projectResult.Unwrap();

        return await accountRepository.UpdateAsync(accountId, account => account.Create(project, credit), cancellationToken);
    }
}
