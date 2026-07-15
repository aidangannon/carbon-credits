using System.Net.Http.Json;
using System.Text.Json;
using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using AutoFixture;
using Core.Models;
using Host.Models;
using LightBDD.XUnit3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Acceptance.Features.Projects;

public partial class Get_Project_By_Id : FeatureFixture
{
    private Guid _projectId;
    private HttpResponseMessage? _httpResponse;
    private Project? _project;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private readonly string _basePath;
    private readonly Fixture _fixture;
    private readonly JsonSerializerOptions _options;
    private const string OperationName = "GetProjectById";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Get_Project_By_Id()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
        _basePath = _services.GetService<IOptions<FileOptions>>()?.Value?.BasePath!;
        _fixture = new Fixture();

        _options = new JsonSerializerOptions { WriteIndented = true };

        _scopes = new Dictionary<string, string>
        {
            [Operation] = OperationName
        };
    }

    private async Task Get_Project_By_Id_ID_Request_Is_Sent(Guid id)
    {
        _scopes[ProjectId] = id.ToString();
        _httpResponse = await _client.GetProjectById(id);
    }

    private async Task A_Project_Exists()
    {
        _project = _fixture.Create<Project>();
        _projectId = _project.Id;

        var projectText = JsonSerializer.Serialize(_project, _options);
        File.WriteAllText(_basePath + $"/projects/{_projectId}", projectText);
    }

    private async Task The_Response_Equals_Project()
    {
        var projectResponse = await _httpResponse!.Content.ReadFromJsonAsync<ProjectResponse>();

        projectResponse!.ShouldEqual(_project!);
    }
}
