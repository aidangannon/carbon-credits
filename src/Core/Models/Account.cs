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
        if (CreatedAt > DateTime.UtcNow)
        {
            return DomainResult.Err(AccountErrors.CreatedInFuture);
        }

        var error = ValidateRetirement(creditId, out var credit);
        if (error is not null)
        {
            return DomainResult.Err(error);
        }

        credit!.RetiredAt = DateTime.UtcNow;
        return DomainResult.Ok();
    }

    public BulkDomainResult RetireCredits(IReadOnlyCollection<Guid> creditIds)
    {
        if (CreatedAt > DateTime.UtcNow)
        {
            return BulkDomainResult.Err(AccountErrors.CreatedInFuture);
        }

        var ids = creditIds.Distinct().ToList();
        var failures = new List<CreditFailure>();
        var credits = new List<Credit>();
        foreach (var id in ids)
        {
            var error = ValidateRetirement(id, out var credit);
            if (error is not null)
            {
                failures.Add(new CreditFailure(id, error));
            }
            else
            {
                credits.Add(credit!);
            }
        }

        if (failures.Count > 0)
        {
            return BulkDomainResult.Err(failures);
        }

        var retiredAt = DateTime.UtcNow;
        foreach (var credit in credits)
        {
            credit.RetiredAt = retiredAt;
        }

        return BulkDomainResult.Ok();
    }

    private string? ValidateRetirement(Guid creditId, out Credit? credit)
    {
        credit = _credits.FirstOrDefault(c => c.Id == creditId);
        if (credit is null)
        {
            return CreditErrors.NotFound;
        }

        if (credit.IssuedAt > DateTime.UtcNow)
        {
            return CreditErrors.IssuedInFuture;
        }

        if (credit.RetiredAt is not null)
        {
            return CreditErrors.AlreadyRetired;
        }

        return null;
    }
}
