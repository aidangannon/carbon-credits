using System.Net.Http.Json;
using System.Text.Json;
using Acceptance.Infrastructure;
using Acceptance.Infrastructure.Extensions;
using AutoFixture;
using AwesomeAssertions;
using Core.Models;
using Host.Models;
using LightBDD.XUnit3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Acceptance.Features.Accounts;

public partial class Retire_Credit : FeatureFixture
{
    private Guid _accountId;
    private Guid _creditId;
    private Credit? _credit;
    private HttpResponseMessage? _httpResponse;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private readonly string _basePath;
    private readonly Fixture _fixture;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string OperationName = "RetireCredit";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Retire_Credit()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
        _basePath = _services.GetService<IOptions<FileOptions>>()?.Value?.BasePath!;
        _fixture = new Fixture();
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        _accountId = Guid.NewGuid();
        _creditId = Guid.NewGuid();

        _scopes = new Dictionary<string, string>
        {
            [Operation] = OperationName
        };
    }

    private Task An_Account_Exists_With_An_Unretired_Credit()
    {
        _credit = _fixture.Build<Credit>()
            .With(c => c.Id, _creditId)
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-1))
            .Create();

        var account = _fixture.Build<Account>()
            .With(a => a.Id, _accountId)
            .With(a => a.Credits, new[] { _credit })
            .Create();

        var accountText = JsonSerializer.Serialize(account, _jsonOptions);
        File.WriteAllText(_basePath + $"/accounts/{_accountId}", accountText);

        return Task.CompletedTask;
    }

    private Task An_Account_Exists_With_An_Already_Retired_Credit()
    {
        _credit = _fixture.Build<Credit>()
            .With(c => c.Id, _creditId)
            .With(c => c.RetiredAt, DateTime.UtcNow.AddDays(-1))
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-2))
            .Create();

        var account = _fixture.Build<Account>()
            .With(a => a.Id, _accountId)
            .With(a => a.Credits, new[] { _credit })
            .Create();

        var accountText = JsonSerializer.Serialize(account, _jsonOptions);
        File.WriteAllText(_basePath + $"/accounts/{_accountId}", accountText);

        return Task.CompletedTask;
    }

    private Task An_Account_Created_In_The_Future_Exists_With_An_Unretired_Credit()
    {
        _credit = _fixture.Build<Credit>()
            .With(c => c.Id, _creditId)
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(-1))
            .Create();

        var account = _fixture.Build<Account>()
            .With(a => a.Id, _accountId)
            .With(a => a.CreatedAt, DateTime.UtcNow.AddDays(1))
            .With(a => a.Credits, new[] { _credit })
            .Create();

        var accountText = JsonSerializer.Serialize(account, _jsonOptions);
        File.WriteAllText(_basePath + $"/accounts/{_accountId}", accountText);

        return Task.CompletedTask;
    }

    private Task An_Account_Exists_With_A_Credit_Issued_In_The_Future()
    {
        _credit = _fixture.Build<Credit>()
            .With(c => c.Id, _creditId)
            .With(c => c.RetiredAt, (DateTime?)null)
            .With(c => c.IssuedAt, DateTime.UtcNow.AddDays(1))
            .Create();

        var account = _fixture.Build<Account>()
            .With(a => a.Id, _accountId)
            .With(a => a.CreatedAt, DateTime.UtcNow.AddDays(-1))
            .With(a => a.Credits, new[] { _credit })
            .Create();

        var accountText = JsonSerializer.Serialize(account, _jsonOptions);
        File.WriteAllText(_basePath + $"/accounts/{_accountId}", accountText);

        return Task.CompletedTask;
    }

    private async Task A_Retire_Credit_Request_Is_Sent(Guid accountId, Guid creditId)
    {
        _scopes[AccountId] = accountId.ToString();
        _scopes[CreditId] = creditId.ToString();
        _httpResponse = await _client.RetireCredit(accountId, creditId);
    }

    private async Task The_Response_Should_Reflect_The_Retired_Credit()
    {
        var creditResponse = await _httpResponse!.Content.ReadFromJsonAsync<CreditResponse>();

        creditResponse!.Id.Should().Be(_creditId);
        creditResponse.RetiredAt.Should().NotBeNull();
        creditResponse.RetiredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
