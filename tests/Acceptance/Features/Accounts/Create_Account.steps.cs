using System.Net.Http.Json;
using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Core.Models;
using Host.Models;
using LightBDD.XUnit3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FileStore;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Acceptance.Features.Accounts;

public partial class Create_Account : FeatureFixture
{
    private HttpResponseMessage? _httpResponse;
    private AccountResponse? _createdAccount;
    private string _name;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private readonly string _basePath;
    private readonly IFileStore _fileStore;
    private const string OperationName = "CreateAccount";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Create_Account()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
        _basePath = _services.GetService<IOptions<FileOptions>>()?.Value?.BasePath!;
        _fileStore = _services.CreateScope().ServiceProvider.GetRequiredService<IFileStore>();
        _name = Guid.NewGuid().ToString();

        _scopes = new Dictionary<string, string>
        {
            [Operation] = OperationName
        };
    }

    private async Task A_Create_Account_Request_Is_Sent(string name)
    {
        _scopes[AccountName] = name;
        _httpResponse = await _client.CreateAccount(new CreateAccountRequest { Name = name });
    }

    private async Task The_Response_Should_Reflect_The_Create_Request()
    {
        _createdAccount = await _httpResponse!.Content.ReadFromJsonAsync<AccountResponse>();

        using var scope = new AssertionScope();
        _createdAccount!.Id.Should().NotBe(Guid.Empty);
        _createdAccount.Name.Should().Be(_name);
        _createdAccount.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _createdAccount.Credits.Should().BeEmpty();
    }

    private async Task The_Account_Should_Be_Persisted()
    {
        var result = await _fileStore.GetAsync<Account>($"{_basePath}/accounts/{_createdAccount!.Id}", CancellationToken.None);
        var account = result.Unwrap();

        using var scope = new AssertionScope();
        account.Id.Should().Be(_createdAccount.Id);
        account.Name.Should().Be(_name);
        account.CreatedAt.Should().Be(_createdAccount.CreatedAt);
        account.Credits.Should().BeEmpty();
    }
}
