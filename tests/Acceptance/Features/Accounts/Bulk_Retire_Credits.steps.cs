using System.Text.Json;
using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using AutoFixture;
using AwesomeAssertions;
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
    private List<Credit> _credits = [];
    private List<Guid> _requestedIds = [];
    private HttpResponseMessage? _httpResponse;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private readonly string _basePath;
    private readonly Fixture _fixture;
    private readonly IFileStore _fileStore;
    private const string OperationName = "BulkRetireCredits";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Bulk_Retire_Credits()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
        _basePath = _services.GetService<IOptions<FileOptions>>()?.Value?.BasePath!;
        _fixture = new Fixture();
        _fileStore = _services.CreateScope().ServiceProvider.GetRequiredService<IFileStore>();

        _accountId = Guid.NewGuid();
        _accountCreatedAt = DateTime.UtcNow.AddDays(-1);

        _scopes = new Dictionary<string, string>
        {
            [Operation] = OperationName
        };
    }

    private Credit NewCredit(DateTime issuedAt, DateTime? retiredAt = null)
    {
        var credit = _fixture.Build<Credit>()
            .With(c => c.IssuedAt, issuedAt)
            .With(c => c.RetiredAt, retiredAt)
            .Create();
        _credits.Add(credit);
        return credit;
    }

    private Task Credits_Exist(params Credit[] credits)
    {
        _requestedIds = credits.Select(c => c.Id).ToList();
        return An_Account_Exists_With_Credits(_credits.ToArray());
    }

    private async Task An_Account_Exists_With_Credits(params Credit[] credits)
    {
        var account = _fixture.Build<Account>()
            .With(a => a.Id, _accountId)
            .With(a => a.CreatedAt, _accountCreatedAt)
            .With(a => a.Credits, credits)
            .Create();

        _fileStore.Add($"{_basePath}/accounts/{_accountId}", account);
        await _fileStore.SaveAsync(CancellationToken.None);
    }

    private Task The_Request_Also_Includes_A_Credit_Not_On_The_Account(Guid creditId)
    {
        _requestedIds.Add(creditId);
        return Task.CompletedTask;
    }

    private async Task A_Bulk_Retire_Request_Is_Sent()
    {
        _scopes[AccountId] = _accountId.ToString();
        _scopes[CreditIds] = string.Join(",", _requestedIds);
        _httpResponse = await _client.BulkRetireCredits(_accountId, new BulkRetireCreditsRequest { CreditIds = _requestedIds });
        await _httpResponse.Content.LoadIntoBufferAsync();
    }

    private async Task All_Requested_Credits_Should_Be_Retired()
    {
        var account = await GetAccount();
        foreach (var id in _requestedIds)
        {
            var retiredAt = account.Credits.Single(c => c.Id == id).RetiredAt;
            retiredAt.Should().NotBeNull();
            retiredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    private async Task Credits_Should_Be_Unchanged()
    {
        var account = await GetAccount();
        foreach (var original in _credits)
        {
            account.Credits.Single(c => c.Id == original.Id).RetiredAt.Should().Be(original.RetiredAt);
        }
    }

    private async Task The_Response_Should_List_Failure_For_Credit_With_Error(Guid creditId, string error)
    {
        var body = await ReadBody();
        var failures = body.GetProperty("failures").EnumerateArray().ToList();
        failures.Should().ContainSingle(f =>
            f.GetProperty("creditId").GetGuid() == creditId && f.GetProperty("error").GetString() == error);
    }

    private async Task The_Response_Should_List_Exactly_N_Failures(int count)
    {
        var body = await ReadBody();
        body.GetProperty("failures").GetArrayLength().Should().Be(count);
    }

    private async Task<JsonElement> ReadBody()
    {
        return JsonSerializer.Deserialize<JsonElement>(await _httpResponse!.Content.ReadAsStringAsync());
    }

    private async Task<Account> GetAccount()
    {
        var result = await _fileStore.GetAsync<Account>($"{_basePath}/accounts/{_accountId}", CancellationToken.None);
        return result.Unwrap();
    }
}
