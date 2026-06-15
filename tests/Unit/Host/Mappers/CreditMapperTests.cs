using AutoFixture;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Core.Models;
using Host.Mappers;

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
}
