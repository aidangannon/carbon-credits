using System.Net.Http.Json;
using System.Text.Json;
using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using AutoFixture;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Core.Models;
using Host.Models;
using LightBDD.XUnit3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Acceptance.Features.Accounts;

public partial class Create_Credit : FeatureFixture
{
    private Guid _accountId;
    private Guid _projectId;
    private DateTime _issuedAt;
    private HttpResponseMessage? _httpResponse;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private readonly string _basePath;
    private readonly Fixture _fixture;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string OperationName = "CreateCredit";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Create_Credit()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
        _basePath = _services.GetService<IOptions<FileOptions>>()?.Value?.BasePath!;
        _fixture = new Fixture();
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        _accountId = Guid.NewGuid();
        _projectId = Guid.NewGuid();
        _issuedAt = DateTime.UtcNow.AddDays(-1);

        _scopes = new Dictionary<string, string>
        {
            [Operation] = OperationName
        };
    }

    private Task An_Account_Exists()
    {
        var account = _fixture.Build<Account>()
            .With(a => a.Id, _accountId)
            .With(a => a.Credits, Array.Empty<Credit>())
            .Create();

        var accountText = JsonSerializer.Serialize(account, _jsonOptions);
        File.WriteAllText(_basePath + $"/accounts/{_accountId}", accountText);

        return Task.CompletedTask;
    }

    private Task A_Project_Exists_For_The_Credit()
    {
        var project = _fixture.Build<Project>()
            .With(p => p.Id, _projectId)
            .Create();

        var projectText = JsonSerializer.Serialize(project, _jsonOptions);
        File.WriteAllText(_basePath + $"/projects/{_projectId}", projectText);

        return Task.CompletedTask;
    }

    private async Task A_Create_Credit_Request_Is_Sent(Guid accountId, DateTime issuedAt, Guid projectId)
    {
        _scopes[AccountId] = accountId.ToString();
        _httpResponse = await _client.CreateCredit(accountId, new CreateCreditRequest
        {
            IssuedAt = issuedAt,
            ProjectId = projectId
        });
    }

    private async Task The_Response_Should_Reflect_The_Created_Credit()
    {
        var creditResponse = await _httpResponse!.Content.ReadFromJsonAsync<CreditResponse>();

        using var scope = new AssertionScope();
        creditResponse!.Id.Should().NotBe(Guid.Empty);
        creditResponse.IssuedAt.Should().BeCloseTo(_issuedAt, TimeSpan.FromSeconds(1));
        creditResponse.ProjectId.Should().Be(_projectId);
        creditResponse.RetiredAt.Should().BeNull();
    }
}
