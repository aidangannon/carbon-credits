using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using LightBDD.XUnit3;

namespace Acceptance.Features;

public partial class Get_Account_By_Id : FeatureFixture
{
    private Guid _accountId;
    private HttpResponseMessage? _httpResponse;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private const string OperationName = "GetAccountById";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Get_Account_By_Id()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
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
}
