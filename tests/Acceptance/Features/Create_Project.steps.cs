using System.Net.Http.Json;
using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Host.Models;
using LightBDD.XUnit3;

namespace Acceptance.Features;

public partial class Create_Project : FeatureFixture
{
    private HttpResponseMessage? _httpResponse;
    private string _name;
    private string _country;
    private string _type;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private const string OperationName = "CreateProject";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Create_Project()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
        _name = Guid.NewGuid().ToString();
        _country = "UK";
        _type = "Renewable";

        _scopes = new Dictionary<string, string>
        {
            [Operation] = OperationName
        };
    }

    private async Task A_Create_Project_Request_Is_Sent(string name, string country, string type)
    {
        _scopes[ProjectName] = name;
        _httpResponse = await _client.CreateProject(new CreateProjectRequest
        {
            Name = name,
            Country = country,
            Type = type
        });
    }

    private async Task The_Response_Should_Reflect_The_Create_Request()
    {
        var projectResponse = await _httpResponse!.Content.ReadFromJsonAsync<ProjectResponse>();

        using var scope = new AssertionScope();
        projectResponse!.Id.Should().NotBe(Guid.Empty);
        projectResponse.Name.Should().Be(_name);
        projectResponse.Country.Should().Be(_country);
        projectResponse.Type.Should().Be(_type);
    }
}
