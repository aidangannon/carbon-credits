using Core.Models;
using FluentValidation;
using Host.Models;

namespace Host.Validators;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.Type).NotEmpty().Must(t => Enum.TryParse<ProjectType>(t, out _)).WithMessage("'Type' must be one of: Renewable, Forestry, Agriculture");
    }
}
