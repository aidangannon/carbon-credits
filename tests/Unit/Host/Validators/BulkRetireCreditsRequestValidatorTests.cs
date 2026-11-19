using AwesomeAssertions;
using Host.Models;
using Host.Validators;

namespace Unit.Host.Validators;

public class BulkRetireCreditsRequestValidatorTests
{
    private readonly BulkRetireCreditsRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WhenCreditIdsProvided_ShouldPass()
    {
        var request = new BulkRetireCreditsRequest { CreditIds = [Guid.NewGuid()] };

        var result = await _validator.ValidateAsync(request, CancellationToken.None);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenCreditIdsEmpty_ShouldFail()
    {
        var request = new BulkRetireCreditsRequest { CreditIds = [] };

        var result = await _validator.ValidateAsync(request, CancellationToken.None);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WhenCreditIdIsEmptyGuid_ShouldFail()
    {
        var request = new BulkRetireCreditsRequest { CreditIds = [Guid.Empty] };

        var result = await _validator.ValidateAsync(request, CancellationToken.None);

        result.IsValid.Should().BeFalse();
    }
}
