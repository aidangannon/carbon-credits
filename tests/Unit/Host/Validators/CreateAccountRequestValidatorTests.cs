using AwesomeAssertions;
using AwesomeAssertions.Execution;
using FluentValidation;
using Host.Models;
using Host.Validators;

namespace Unit.Host.Validators;

public class CreateAccountRequestValidatorTests
{
    private readonly IValidator<CreateAccountRequest> _validator = new CreateAccountRequestValidator();

    [Fact]
    public async Task Validate_WhenNameIsValid_ShouldPass()
    {
        var request = new CreateAccountRequest { Name = "Test Account" };

        var result = await _validator.ValidateAsync(request, CancellationToken.None);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ShouldFail()
    {
        var request = new CreateAccountRequest { Name = string.Empty };

        var result = await _validator.ValidateAsync(request, CancellationToken.None);

        using var scope = new AssertionScope();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateAccountRequest.Name));
    }

    [Fact]
    public async Task Validate_WhenNameIsWhitespace_ShouldFail()
    {
        var request = new CreateAccountRequest { Name = "   " };

        var result = await _validator.ValidateAsync(request, CancellationToken.None);

        using var scope = new AssertionScope();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateAccountRequest.Name));
    }
}
