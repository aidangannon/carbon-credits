using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Core.Models;
using Host.Models;

namespace Acceptance.Infrastructure.Extensions;

public static class CreditResponseAssertions
{
    public static void ShouldEqual(this CreditResponse response, Credit credit)
    {
        using var scope = new AssertionScope();
        response.Id.Should().Be(credit.Id);
        response.IssuedAt.Should().Be(credit.IssuedAt);
        response.ProjectId.Should().Be(credit.ProjectId);
        response.RetiredAt.Should().Be(credit.RetiredAt);
    }
}
