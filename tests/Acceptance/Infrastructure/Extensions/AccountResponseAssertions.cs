using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Core.Models;
using Host.Models;

namespace Acceptance.Infrastructure.Extensions;

public static class AccountResponseAssertions
{
    public static void ShouldEqual(this AccountResponse response, Account account)
    {
        using var scope = new AssertionScope();
        response.Id.Should().Be(account.Id);
        response.Name.Should().Be(account.Name);
        response.CreatedAt.Should().Be(account.CreatedAt);
        response.Credits.Should().HaveCount(account.Credits.Count);
        response.Credits
            .Zip(account.Credits, (creditResponse, credit) => (creditResponse, credit))
            .ToList()
            .ForEach(pair => pair.creditResponse.ShouldEqual(pair.credit));
    }
}
