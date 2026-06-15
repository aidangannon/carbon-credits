using AutoFixture;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Core.Models;
using Host.Mappers;

namespace Unit.Host.Mappers;

public class AccountMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToResponse_ShouldMapAllFields()
    {
        var credit = _fixture.Create<Credit>();
        var account = _fixture
            .Build<Account>()
            .With(a => a.Credits, [credit])
            .Create();

        var response = account.ToResponse();

        using var scope = new AssertionScope();
        response.Id.Should().Be(account.Id);
        response.Name.Should().Be(account.Name);
        response.CreatedAt.Should().Be(account.CreatedAt);
        response.Credits.Should().HaveCount(1);
        response.Credits.First().Id.Should().Be(credit.Id);
    }

    [Fact]
    public void ToResponse_WhenCreditsIsEmpty_ShouldMapAsEmpty()
    {
        var account = _fixture
            .Build<Account>()
            .With(a => a.Credits, [])
            .Create();

        account
            .ToResponse()
            .Credits
            .Should()
            .BeEmpty();
    }
}
