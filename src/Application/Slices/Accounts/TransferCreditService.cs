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
            return accountResult;
        }

        var account = accountResult.Unwrap();

        var recipientResult = await accountRepository.GetByIdAsync(recipientAccountId, cancellationToken);
        if (recipientResult.HasFailed())
        {
            return recipientResult;
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
        // save flushes both mutations together (no per-entity ordering control anymore). If it still
        // fails, the two records may be left inconsistent - this is an unrecoverable, exceptional
        // condition rather than a normal domain error, so we throw and let the global exception
        // handler surface it as a 500.
        var saveResult = await accountRepository.SaveAsync(cancellationToken);
        if (saveResult.HasFailed())
        {
            throw new InvalidOperationException(
                $"Account id: {accountId} and account id: {recipientAccountId} have been left in a partial transfer state: {saveResult.Error}");
        }

        return Result<Account>.Ok(account);
    }
}
