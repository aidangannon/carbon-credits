using AutoFixture;
using AwesomeAssertions;
using Core.Errors;
using Core.Models;

namespace Unit.Core.Models;

public class AccountTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void CanCreate_WhenCreditIsRetired_ShouldReturnCannotCreateRetiredError()
    {
        var project = _fixture.Create<Project>();
        var credit = _fixture
            .Build<Credit>()
            .With(c => c.ProjectId, project.Id)
            .With(c => c.RetiredAt, DateTime.UtcNow)
            .Create();
        var account = BuildAccount();

        var result = account.CanCreate(project, credit);

        result.Error.Should().Be(CreditErrors.CannotCreateRetired);
    }

    [Fact]
    public void CanCreate_WhenIssuedInFuture_ShouldReturnIssuedInFutureError()
    {
        var project = _fixture.Create<Project>();
        var credit = _fixture
            .Build<Credit>()
            .With(c => c.ProjectId, project.Id)
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(1))
            .Create();
        var account = BuildAccount();

        var result = account.CanCreate(project, credit);

        result.Error.Should().Be(CreditErrors.IssuedInFuture);
    }

    [Fact]
    public void CanCreate_WhenProjectIdMismatches_ShouldReturnProjectMismatchError()
    {
        var project = _fixture.Create<Project>();
        var credit = _fixture
            .Build<Credit>()
            .With(c => c.ProjectId, Guid.NewGuid())
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-1))
            .Create();
        var account = BuildAccount();

        var result = account.CanCreate(project, credit);

        result.Error.Should().Be(CreditErrors.ProjectMismatch);
    }

    [Fact]
    public void CanCreate_WhenValid_ShouldReturnOk()
    {
        var project = _fixture.Create<Project>();
        var credit = _fixture
            .Build<Credit>()
            .With(c => c.ProjectId, project.Id)
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-1))
            .Create();
        var account = BuildAccount();

        var result = account.CanCreate(project, credit);

        result.HasFailed().Should().BeFalse();
    }

    private Account BuildAccount() =>
        _fixture
            .Build<Account>()
            .With(a => a.Credits, Array.Empty<Credit>())
            .Create();
}
