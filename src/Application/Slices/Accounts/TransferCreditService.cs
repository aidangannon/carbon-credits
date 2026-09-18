using Application.Ports;
using Core.Errors;
using Core.Models;
using Crosscutting.Result;

namespace Application.Slices.Accounts;

public interface ICreditTransferService
{
    Task<Result<Account>> Transfer(Guid accountId, Guid recipientAccountId, Guid creditId, CancellationToken cancellationToken);
}

public class CreditTransferService(IAccountRepository accountRepository, IProjectRepository projectRepository) : ICreditTransferService
{
    public async Task<Result<Account>> Transfer(Guid accountId, Guid recipientAccountId, Guid creditId, CancellationToken cancellationToken)
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

        var saveResult = await accountRepository.SaveAsync(cancellationToken);
        if (saveResult.HasFailed())
        {
            throw new InvalidOperationException(
                $"Account id: {accountId} and account id: {recipientAccountId} have been left in a partial transfer state: {saveResult.Error}");
        }

        return Result<Account>.Ok(account);
    }
}
