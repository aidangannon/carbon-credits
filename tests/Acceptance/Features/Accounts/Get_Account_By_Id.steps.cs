using System.Net.Http.Json;
using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using AutoFixture;
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

    private async Task An_Account_Exists()
    {
        _account = _fixture.Create<Account>();
        _accountId = _account.Id;

        await _fileStore.SaveAsync($"{_basePath}/accounts/{_accountId}", _account, CancellationToken.None);
    }

    private async Task The_Response_Equals_Account()
    {
        var accountResponse = await _httpResponse!.Content.ReadFromJsonAsync<AccountResponse>();

        accountResponse!.ShouldEqual(_account!);
    }
}
