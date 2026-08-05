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

    public DomainResult Create(Project project, Credit credit)
    {
        if (credit.RetiredAt is not null)
            return DomainResult.Err(CreditErrors.CannotCreateRetired);

        if (credit.IssuedAt > DateTime.UtcNow)
            return DomainResult.Err(CreditErrors.IssuedInFuture);

        if (credit.ProjectId != project.Id)
            return DomainResult.Err(CreditErrors.ProjectMismatch);

        _credits.Add(credit);
        return DomainResult.Ok();
    }
}
