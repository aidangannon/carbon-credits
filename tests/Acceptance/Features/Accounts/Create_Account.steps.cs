using System.Net.Http.Json;
using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Host.Models;
using LightBDD.XUnit3;

namespace Acceptance.Features.Accounts;

public partial class Create_Account : FeatureFixture
{
    private HttpResponseMessage? _httpResponse;
    private string _name;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private const string OperationName = "CreateAccount";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Create_Account()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
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
        var accountResponse = await _httpResponse!.Content.ReadFromJsonAsync<AccountResponse>();

        using var scope = new AssertionScope();
        accountResponse!.Id.Should().NotBe(Guid.Empty);
        accountResponse.Name.Should().Be(_name);
        accountResponse.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        accountResponse.Credits.Should().BeEmpty();
    }
}
