using AutoFixture;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Core.Models;
using Host.Mappers.Accounts;
using Host.Models;

namespace Unit.Host.Mappers;

public class AccountMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToResponse_ShouldMapAllFields()
    {
        var credit = _fixture
            .Build<Credit>()
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-1))
            .Create();
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

    [Fact]
    public void ToResponse_WhenIncludeRetiredCreditsIsFalse_ShouldExcludeRetiredCredits()
    {
        var retiredCredit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, DateTime.UtcNow.AddDays(-1))
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-2))
            .Create();
        var activeCredit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-2))
            .Create();
        var account = _fixture
            .Build<Account>()
            .With(a => a.Credits, [retiredCredit, activeCredit])
            .Create();

        var response = account.ToResponse(includeRetiredCredits: false, includeFutureCredits: true);

        response.Credits.Should().ContainSingle(c => c.Id == activeCredit.Id);
    }

    [Fact]
    public void ToResponse_WhenIncludeRetiredCreditsIsTrue_ShouldIncludeRetiredCredits()
    {
        var retiredCredit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, DateTime.UtcNow.AddDays(-1))
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-2))
            .Create();
        var account = _fixture
            .Build<Account>()
            .With(a => a.Credits, [retiredCredit])
            .Create();

        var response = account.ToResponse(includeRetiredCredits: true, includeFutureCredits: true);

        response.Credits.Should().ContainSingle(c => c.Id == retiredCredit.Id);
    }

    [Fact]
    public void ToResponse_WhenIncludeFutureCreditsIsFalse_ShouldExcludeFutureCredits()
    {
        var futureCredit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(1))
            .Create();
        var pastCredit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-1))
            .Create();
        var account = _fixture
            .Build<Account>()
            .With(a => a.Credits, [futureCredit, pastCredit])
            .Create();

        var response = account.ToResponse(includeRetiredCredits: true, includeFutureCredits: false);

        response.Credits.Should().ContainSingle(c => c.Id == pastCredit.Id);
    }

    [Fact]
    public void ToResponse_WhenIncludeFutureCreditsIsTrue_ShouldIncludeFutureCredits()
    {
        var futureCredit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(1))
            .Create();
        var account = _fixture
            .Build<Account>()
            .With(a => a.Credits, [futureCredit])
            .Create();

        var response = account.ToResponse(includeRetiredCredits: true, includeFutureCredits: true);

        response.Credits.Should().ContainSingle(c => c.Id == futureCredit.Id);
    }

    [Fact]
    public void ToResponse_WhenCalledWithDefaults_ShouldIncludeRetiredAndExcludeFutureCredits()
    {
        var retiredCredit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, DateTime.UtcNow.AddDays(-1))
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-2))
            .Create();
        var futureCredit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(1))
            .Create();
        var account = _fixture
            .Build<Account>()
            .With(a => a.Credits, [retiredCredit, futureCredit])
            .Create();

        var response = account.ToResponse();

        response.Credits.Should().ContainSingle(c => c.Id == retiredCredit.Id);
    }

    [Fact]
    public void ToAccount_ShouldMapAllFields()
    {
        var request = new CreateAccountRequest { Name = "Test Account" };

        var account = request.ToAccount();

        using var scope = new AssertionScope();
        account.Id.Should().NotBe(Guid.Empty);
        account.Name.Should().Be(request.Name);
        account.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        account.Credits.Should().BeEmpty();
    }
}
