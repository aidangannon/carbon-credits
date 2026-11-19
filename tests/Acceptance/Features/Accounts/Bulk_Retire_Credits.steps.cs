using System.Net.Http.Json;
using System.Text.Json;
using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using AutoFixture;
using AwesomeAssertions;
using Core.Errors;
using Core.Models;
using Host.Models;
using LightBDD.XUnit3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FileStore;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Acceptance.Features.Accounts;

public partial class Bulk_Retire_Credits : FeatureFixture
{
    private Guid _accountId;
    private DateTime _accountCreatedAt;
    private readonly Guid[] _validIds = [Guid.NewGuid(), Guid.NewGuid()];
    private readonly Guid _futureId = Guid.NewGuid();
    private readonly Guid _retiredId = Guid.NewGuid();
    private readonly Guid _missingId = Guid.NewGuid();
    private HttpResponseMessage? _httpResponse;
    private readonly HttpClient _client;
    private readonly string _basePath;
    private readonly Fixture _fixture;
    private readonly IFileStore _fileStore;

    public Bulk_Retire_Credits()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        var services = TestWebApplicationFactory.Instance!.Services;
        _basePath = services.GetService<IOptions<FileOptions>>()?.Value?.BasePath!;
        _fixture = new Fixture();
        _fileStore = services.CreateScope().ServiceProvider.GetRequiredService<IFileStore>();

        _accountId = Guid.NewGuid();
        _accountCreatedAt = DateTime.UtcNow.AddDays(-1);
    }

    private async Task An_Account_Exists_With_Credits()
    {
        Credit Build(Guid id, DateTime issuedAt, DateTime? retiredAt) => _fixture.Build<Credit>()
            .With(c => c.Id, id)
            .With(c => c.IssuedAt, issuedAt)
            .With(c => c.RetiredAt, retiredAt)
            .Create();

        var past = DateTime.UtcNow.AddDays(-1);
        var credits = new[]
        {
            Build(_validIds[0], past, null),
            Build(_validIds[1], past, null),
            Build(_futureId, DateTime.UtcNow.AddDays(1), null),
            Build(_retiredId, past, past)
        };

        var account = _fixture.Build<Account>()
            .With(a => a.Id, _accountId)
            .With(a => a.CreatedAt, _accountCreatedAt)
            .With(a => a.Credits, credits)
            .Create();

        _fileStore.Add($"{_basePath}/accounts/{_accountId}", account);
        await _fileStore.SaveAsync(CancellationToken.None);
    }

    private async Task A_Bulk_Retire_Request_Is_Sent(Guid accountId, Guid[] creditIds)
    {
        _httpResponse = await _client.BulkRetireCredits(accountId, new BulkRetireCreditsRequest { CreditIds = creditIds });
    }

    private async Task The_Response_Should_List_The_Failed_Credits()
    {
        var body = await _httpResponse!.Content.ReadFromJsonAsync<JsonElement>();
        var failures = body.GetProperty("failures").EnumerateArray()
            .ToDictionary(f => f.GetProperty("creditId").GetGuid(), f => f.GetProperty("error").GetString());

        failures.Should().BeEquivalentTo(new Dictionary<Guid, string?>
        {
            [_futureId] = CreditErrors.IssuedInFuture,
            [_retiredId] = CreditErrors.AlreadyRetired,
            [_missingId] = CreditErrors.NotFound
        });
    }

    private async Task<Account> GetAccount()
    {
        var result = await _fileStore.GetAsync<Account>($"{_basePath}/accounts/{_accountId}", CancellationToken.None);
        return result.Unwrap();
    }

    private async Task The_Valid_Credits_Should_Not_Be_Retired()
    {
        var account = await GetAccount();
        account.Credits.Where(c => _validIds.Contains(c.Id)).Should().OnlyContain(c => c.RetiredAt == null);
    }

    private async Task The_Valid_Credits_Should_Be_Retired()
    {
        var account = await GetAccount();
        var credits = account.Credits.Where(c => _validIds.Contains(c.Id)).ToList();
        credits.Should().HaveCount(2);
        credits.Should().OnlyContain(c => c.RetiredAt != null);
        credits.Select(c => c.RetiredAt!.Value).Should().AllSatisfy(r => r.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5)));
    }
}
