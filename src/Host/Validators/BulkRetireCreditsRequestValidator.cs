using FluentValidation;
using Host.Models;

namespace Host.Validators;

public class BulkRetireCreditsRequestValidator : AbstractValidator<BulkRetireCreditsRequest>
{
    public BulkRetireCreditsRequestValidator()
    {
        RuleFor(x => x.CreditIds).NotEmpty();
        RuleForEach(x => x.CreditIds).NotEqual(Guid.Empty);
    }
}
