using System.Net.Http.Json;
using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using AutoFixture;
using AwesomeAssertions;
using Core.Models;
using Host.Models;
using LightBDD.XUnit3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Acceptance.Features.Accounts;

public partial class Retire_Credit : FeatureFixture
{
    private Guid _accountId;
    private Guid _creditId;
    private DateTime _accountCreatedAt;
    private DateTime _creditIssuedAt;
    private DateTime? _creditRetiredAt;
    private Credit? _credit;
    private HttpResponseMessage? _httpResponse;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private readonly string _basePath;
    private readonly Fixture _fixture;
    private readonly IFileStore _fileStore;
    private const string OperationName = "RetireCredit";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Retire_Credit()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
        _basePath = _services.GetService<IOptions<FileOptions>>()?.Value?.BasePath!;
        _fixture = new Fixture();
        _fileStore = _services.GetRequiredService<IFileStore>();

        _accountId = Guid.NewGuid();
        _creditId = Guid.NewGuid();
        _accountCreatedAt = DateTime.UtcNow.AddDays(-1);
        _creditIssuedAt = DateTime.UtcNow.AddDays(-1);
        _creditRetiredAt = null;

        _scopes = new Dictionary<string, string>
        {
            [Operation] = OperationName
        };
    }

    private async Task An_Account_Exists()
    {
        _credit = _fixture.Build<Credit>()
            .With(c => c.Id, _creditId)
            .With(c => c.IssuedAt, _creditIssuedAt)
            .With(c => c.RetiredAt, _creditRetiredAt)
            .Create();

        var account = _fixture.Build<Account>()
            .With(a => a.Id, _accountId)
            .With(a => a.CreatedAt, _accountCreatedAt)
            .With(a => a.Credits, new[] { _credit })
            .Create();

        await _fileStore.SaveAsync($"{_basePath}/accounts/{_accountId}", account, CancellationToken.None);
    }

    private async Task An_Account_Exists_With_No_Credits()
    {
        var account = _fixture.Build<Account>()
            .With(a => a.Id, _accountId)
            .With(a => a.CreatedAt, _accountCreatedAt)
            .With(a => a.Credits, Array.Empty<Credit>())
            .Create();

        await _fileStore.SaveAsync($"{_basePath}/accounts/{_accountId}", account, CancellationToken.None);
    }

    private async Task A_Retire_Credit_Request_Is_Sent(Guid accountId, Guid creditId)
    {
        _scopes[AccountId] = accountId.ToString();
        _scopes[CreditId] = creditId.ToString();
        _httpResponse = await _client.RetireCredit(accountId, creditId);
    }

    private async Task The_Credit_Should_Be_Retired_On_The_Account()
    {
        var getResponse = await _client.GetAccountById(_accountId);
        var accountResponse = await getResponse.Content.ReadFromJsonAsync<AccountResponse>();

        var creditResponse = accountResponse!.Credits.Single(c => c.Id == _creditId);
        creditResponse.ProjectId.Should().Be(_credit!.ProjectId);
        creditResponse.IssuedAt.Should().Be(_credit.IssuedAt);
        creditResponse.RetiredAt.Should().NotBeNull();
        creditResponse.RetiredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
