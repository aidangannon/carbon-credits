using AutoFixture;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Core.Models;
using Host.Mappers.Accounts;
using Host.Models;

namespace Unit.Host.Mappers;

public class CreditMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToResponse_ShouldMapAllFields()
    {
        var credit = _fixture.Create<Credit>();

        var response = credit.ToResponse();

        using var scope = new AssertionScope();
        response.Id.Should().Be(credit.Id);
        response.IssuedAt.Should().Be(credit.IssuedAt);
        response.ProjectId.Should().Be(credit.ProjectId);
        response.RetiredAt.Should().Be(credit.RetiredAt);
    }

    [Fact]
    public void ToResponse_WhenRetiredAtIsNull_ShouldMapAsNull()
    {
        var credit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, (DateTime?)null)
            .Create();

        credit
            .ToResponse()
            .RetiredAt
            .Should()
            .BeNull();
    }

    [Fact]
    public void ToCredit_ShouldMapAllFields()
    {
        var request = _fixture.Create<CreateCreditRequest>();

        var credit = request.ToCredit();

        using var scope = new AssertionScope();
        credit.Id.Should().NotBe(Guid.Empty);
        credit.IssuedAt.Should().Be(request.IssuedAt);
        credit.ProjectId.Should().Be(request.ProjectId);
        credit.RetiredAt.Should().BeNull();
    }
}
