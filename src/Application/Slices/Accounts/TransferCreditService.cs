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

        var saveAccountResult = await accountRepository.SaveAsync(account, cancellationToken);
        if (saveAccountResult.HasFailed())
        {
            return Result<Account>.Err(saveAccountResult.Error);
        }

        var saveRecipientResult = await accountRepository.SaveAsync(recipient, cancellationToken);

        return saveRecipientResult.HasFailed() ? Result<Account>.Err(saveRecipientResult.Error) : Result<Account>.Ok(account);
    }
}
