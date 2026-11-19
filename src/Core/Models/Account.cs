using Core.Errors;
using Core.Result;

namespace Core.Models;

public class Account
{
    private readonly List<Credit> _credits = [];

    public required Guid Id { init; get; }
    public required string Name { set; get; }
    public required DateTime CreatedAt { set; get; }
    public required IReadOnlyCollection<Credit> Credits { init => _credits = [.. value]; get => _credits; }

    public DomainResult Transfer(Account recipient, Guid creditId)
    {
        var credit = _credits.FirstOrDefault(c => c.Id == creditId);
        if (credit is null)
        {
            return DomainResult.Err(CreditErrors.NotFound);
        }

        if (CreatedAt > DateTime.UtcNow || recipient.CreatedAt > DateTime.UtcNow)
        {
            return DomainResult.Err(AccountErrors.CreatedInFuture);
        }

        if (credit.IssuedAt > DateTime.UtcNow)
        {
            return DomainResult.Err(CreditErrors.IssuedInFuture);
        }

        _credits.Remove(credit);
        recipient._credits.Add(credit);

        return DomainResult.Ok();
    }

    public DomainResult AddCredit(Project project, Credit credit)
    {
        if (credit.RetiredAt is not null)
        {
            return DomainResult.Err(CreditErrors.CannotCreateRetired);
        }

        if (credit.IssuedAt > DateTime.UtcNow)
        {
            return DomainResult.Err(CreditErrors.IssuedInFuture);
        }

        if (credit.ProjectId != project.Id)
        {
            return DomainResult.Err(CreditErrors.ProjectMismatch);
        }

        _credits.Add(credit);
        return DomainResult.Ok();
    }

    public IReadOnlyCollection<Credit> GetCredits(bool includeRetiredCredits, bool includeFutureCredits)
    {
        return _credits
            .Where(c => includeRetiredCredits || c.RetiredAt is null)
            .Where(c => includeFutureCredits || c.IssuedAt <= DateTime.UtcNow)
            .ToList();
    }

    public DomainResult RetireCredit(Guid creditId)
    {
        var validation = ValidateRetirement(creditId);
        if (validation.Error is not null)
        {
            return DomainResult.Err(validation.Error);
        }

        validation.Credit!.RetiredAt = DateTime.UtcNow;
        return DomainResult.Ok();
    }

    public BulkRetireCreditsResult RetireCredits(IReadOnlyCollection<Guid> creditIds)
    {
        var ids = creditIds.Distinct().ToList();

        var accountError = ValidateAccount();
        if (accountError is not null)
        {
            return BulkRetireCreditsResult.AccountError(accountError);
        }

        var checks = ids.Select(id => (Id: id, Result: ValidateRetirement(id))).ToList();
        var failures = checks
            .Where(c => c.Result.Error is not null)
            .Select(c => new CreditRetirementFailure(c.Id, c.Result.Error!))
            .ToList();

        if (failures.Count > 0)
        {
            return BulkRetireCreditsResult.CreditFailures(failures);
        }

        var retiredAt = DateTime.UtcNow;
        foreach (var check in checks)
        {
            check.Result.Credit!.RetiredAt = retiredAt;
        }

        return BulkRetireCreditsResult.Ok();
    }

    private string? ValidateAccount()
    {
        return CreatedAt > DateTime.UtcNow ? AccountErrors.CreatedInFuture : null;
    }

    private (Credit? Credit, string? Error) ValidateRetirement(Guid creditId)
    {
        var accountError = ValidateAccount();
        if (accountError is not null)
        {
            return (null, accountError);
        }

        var credit = _credits.FirstOrDefault(c => c.Id == creditId);
        if (credit is null)
        {
            return (null, CreditErrors.NotFound);
        }

        if (credit.IssuedAt > DateTime.UtcNow)
        {
            return (null, CreditErrors.IssuedInFuture);
        }

        if (credit.RetiredAt is not null)
        {
            return (null, CreditErrors.AlreadyRetired);
        }

        return (credit, null);
    }
}
