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
        var accountCheck = await accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (accountCheck.HasFailed())
        {
            return Result<Account>.Err(accountCheck.Error);
        }

        var projectResult = await projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (projectResult.HasFailed())
        {
            return Result<Account>.Err(projectResult.Error);
        }

        var project = projectResult.Unwrap();

        return await accountRepository.AddCreditAsync(accountId, project, credit, cancellationToken);
    }
}
