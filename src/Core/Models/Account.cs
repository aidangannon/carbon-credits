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

    public void Transfer(Account recipient, Guid creditId)
    {
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

    public DomainResult RetireCredit(Guid creditId)
    {
        if (CreatedAt > DateTime.UtcNow)
        {
            return DomainResult.Err(AccountErrors.CreatedInFuture);
        }

        var credit = _credits.FirstOrDefault(c => c.Id == creditId);
        if (credit is null)
        {
            return DomainResult.Err(CreditErrors.NotFound);
        }

        if (credit.IssuedAt > DateTime.UtcNow)
        {
            return DomainResult.Err(CreditErrors.IssuedInFuture);
        }

        if (credit.RetiredAt is not null)
        {
            return DomainResult.Err(CreditErrors.AlreadyRetired);
        }

        credit.RetiredAt = DateTime.UtcNow;
        return DomainResult.Ok();
    }
}
