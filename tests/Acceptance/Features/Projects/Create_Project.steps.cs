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

namespace Acceptance.Features.Projects;

public partial class Create_Project : FeatureFixture
{
    private HttpResponseMessage? _httpResponse;
    private ProjectResponse? _createdProject;
    private string _name;
    private string _country;
    private string _type;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private readonly string _basePath;
    private readonly IFileStore _fileStore;
    private const string OperationName = "CreateProject";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Create_Project()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
        _basePath = _services.GetService<IOptions<FileOptions>>()?.Value?.BasePath!;
        _fileStore = _services.CreateScope().ServiceProvider.GetRequiredService<IFileStore>();
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
        _createdProject = await _httpResponse!.Content.ReadFromJsonAsync<ProjectResponse>();

        using var scope = new AssertionScope();
        _createdProject!.Id.Should().NotBe(Guid.Empty);
        _createdProject.Name.Should().Be(_name);
        _createdProject.Country.Should().Be(_country);
        _createdProject.Type.Should().Be(_type);
    }

    private async Task The_Project_Should_Be_Persisted()
    {
        var result = await _fileStore.GetAsync<Project>($"{_basePath}/projects/{_createdProject!.Id}", CancellationToken.None);
        var project = result.Unwrap();

        using var scope = new AssertionScope();
        project.Id.Should().Be(_createdProject.Id);
        project.Name.Should().Be(_name);
        project.Country.Should().Be(_country);
        project.Type.ToString().Should().Be(_type);
    }
}
