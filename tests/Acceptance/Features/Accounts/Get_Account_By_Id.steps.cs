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

public partial class Get_Account_By_Id : FeatureFixture
{
    private Guid _accountId;
    private Guid _retiredCreditId;
    private Guid _futureCreditId;
    private HttpResponseMessage? _httpResponse;
    private Account? _account;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private readonly string _basePath;
    private readonly Fixture _fixture;
    private readonly IFileStore _fileStore;
    private const string OperationName = "GetAccountById";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Get_Account_By_Id()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
        _basePath = _services.GetService<IOptions<FileOptions>>()?.Value?.BasePath!;
        _fixture = new Fixture();
        _fileStore = _services.GetRequiredService<IFileStore>();

        _scopes = new Dictionary<string, string>()
        {
            [Operation] = OperationName
        };
    }

    private async Task Get_Account_By_Id_ID_Request_Is_Sent(Guid id)
    {
        _scopes[AccountId] = id.ToString();
        _httpResponse = await _client.GetAccountById(id);
    }

    private async Task Get_Account_By_Id_ID_Request_Is_Sent_With_Filters(Guid id, bool? includeRetiredCredits, bool? includeFutureCredits)
    {
        _scopes[AccountId] = id.ToString();
        _httpResponse = await _client.GetAccountById(id, includeRetiredCredits, includeFutureCredits);
    }

    private async Task An_Account_Exists()
    {
        var credit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-1))
            .Create();

        _account = _fixture
            .Build<Account>()
            .With(a => a.Credits, [credit])
            .Create();
        _accountId = _account.Id;

        await _fileStore.SaveAsync($"{_basePath}/accounts/{_accountId}", _account, CancellationToken.None);
    }

    private async Task An_Account_Exists_With_A_Retired_Credit()
    {
        var retiredCredit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, DateTime.UtcNow.AddDays(-1))
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-2))
            .Create();
        _retiredCreditId = retiredCredit.Id;

        _account = _fixture
            .Build<Account>()
            .With(a => a.Credits, [retiredCredit])
            .Create();
        _accountId = _account.Id;

        await _fileStore.SaveAsync($"{_basePath}/accounts/{_accountId}", _account, CancellationToken.None);
    }

    private async Task An_Account_Exists_With_A_Future_Credit()
    {
        var futureCredit = _fixture
            .Build<Credit>()
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(1))
            .Create();
        _futureCreditId = futureCredit.Id;

        _account = _fixture
            .Build<Account>()
            .With(a => a.Credits, [futureCredit])
            .Create();
        _accountId = _account.Id;

        await _fileStore.SaveAsync($"{_basePath}/accounts/{_accountId}", _account, CancellationToken.None);
    }

    private async Task The_Response_Equals_Account()
    {
        var accountResponse = await _httpResponse!.Content.ReadFromJsonAsync<AccountResponse>();

        accountResponse!.ShouldEqual(_account!);
    }

    private async Task The_Response_Should_Include_The_Credit_ID(Guid creditId)
    {
        var accountResponse = await _httpResponse!.Content.ReadFromJsonAsync<AccountResponse>();

        accountResponse!.Credits.Should().Contain(c => c.Id == creditId);
    }

    private async Task The_Response_Should_Not_Include_The_Credit_ID(Guid creditId)
    {
        var accountResponse = await _httpResponse!.Content.ReadFromJsonAsync<AccountResponse>();

        accountResponse!.Credits.Should().NotContain(c => c.Id == creditId);
    }
}

