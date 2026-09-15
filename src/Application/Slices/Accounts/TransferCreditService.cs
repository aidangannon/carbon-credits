using Application.Ports;
using Core.Errors;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Accounts;

public interface ITransferCreditService
{
    Task<Result<Account>> TransferCredit(Guid accountId, Guid recipientAccountId, Guid creditId, CancellationToken cancellationToken);
}

public class TransferCreditService(IAccountRepository accountRepository, IProjectRepository projectRepository) : ITransferCreditService
{
    public async Task<Result<Account>> TransferCredit(Guid accountId, Guid recipientAccountId, Guid creditId, CancellationToken cancellationToken)
    {
        var accountResult = await accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (accountResult.HasFailed())
        {
            return Result<Account>.Err(accountResult.Error);
        }

        var account = accountResult.Unwrap();

        var recipientResult = await accountRepository.GetByIdAsync(recipientAccountId, cancellationToken);
        if (recipientResult.HasFailed())
        {
            return Result<Account>.Err(recipientResult.Error);
        }

        var recipient = recipientResult.Unwrap();

        var credit = account.Credits.FirstOrDefault(c => c.Id == creditId);
        if (credit is not null)
        {
            var projectResult = await projectRepository.GetByIdAsync(credit.ProjectId, cancellationToken);
            if (projectResult.HasFailed())
            {
                return Result<Account>.Err(CreditErrors.ProjectNotFoundMustRetire);
            }
        }

        var domainResult = account.Transfer(recipient, creditId);
        if (domainResult.HasFailed())
        {
            return Result<Account>.Err(domainResult.Error!);
        }

        // Both accounts were tracked by their GetByIdAsync loads above, so this single unit-of-work
        // save flushes both mutations together (no per-entity ordering control anymore).
        var saveResult = await accountRepository.SaveAsync(cancellationToken);
        if (saveResult.HasFailed())
        {
            return Result<Account>.Err(saveResult.Error);
        }

        return Result<Account>.Ok(account);
    }
}
