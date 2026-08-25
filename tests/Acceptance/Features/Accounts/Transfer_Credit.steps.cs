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

public partial class Transfer_Credit : FeatureFixture
{
    private Guid _accountId;
    private Guid _recipientAccountId;
    private Guid _creditId;
    private Guid _projectId;
    private DateTime _issuedAt;
    private HttpResponseMessage? _httpResponse;
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _scopes;
    private readonly IServiceProvider _services;
    private readonly string _basePath;
    private readonly Fixture _fixture;
    private readonly IFileStore _fileStore;
    private const string OperationName = "TransferCredit";
    private const string EndpointCalledMessage = "Endpoint Called";
    private const string EndpointCompletedMessage = "Endpoint Completed";

    public Transfer_Credit()
    {
        _client = TestWebApplicationFactory.Instance!.CreateClient();
        _services = TestWebApplicationFactory.Instance!.Services;
        _basePath = _services.GetService<IOptions<FileOptions>>()?.Value?.BasePath!;
        _fixture = new Fixture();
        _fileStore = _services.GetRequiredService<IFileStore>();

        _accountId = Guid.NewGuid();
        _recipientAccountId = Guid.NewGuid();
        _creditId = Guid.NewGuid();
        _projectId = Guid.NewGuid();
        _issuedAt = DateTime.UtcNow.AddDays(-1);

        _scopes = new Dictionary<string, string>
        {
            [Operation] = OperationName
        };
    }

    private async Task An_Account_Exists()
    {
        var account = _fixture.Build<Account>()
            .With(a => a.Id, _accountId)
            .With(a => a.CreatedAt, DateTime.UtcNow.AddDays(-1))
            .With(a => a.Credits, Array.Empty<Credit>())
            .Create();

        await _fileStore.SaveAsync($"{_basePath}/accounts/{_accountId}", account, CancellationToken.None);
    }

    private Task A_Recipient_Account_Exists()
    {
        return A_Recipient_Account_Exists_Created_At(DateTime.UtcNow.AddDays(-1));
    }

    private async Task A_Recipient_Account_Exists_Created_At(DateTime createdAt)
    {
        var account = _fixture.Build<Account>()
            .With(a => a.Id, _recipientAccountId)
            .With(a => a.CreatedAt, createdAt)
            .With(a => a.Credits, Array.Empty<Credit>())
            .Create();

        await _fileStore.SaveAsync($"{_basePath}/accounts/{_recipientAccountId}", account, CancellationToken.None);
    }

    private async Task A_Project_Exists_For_The_Credit()
    {
        var project = _fixture.Build<Project>()
            .With(p => p.Id, _projectId)
            .Create();

        await _fileStore.SaveAsync($"{_basePath}/projects/{_projectId}", project, CancellationToken.None);
    }

    private async Task A_Credit_Exists_On_The_Account()
    {
        var credit = _fixture.Build<Credit>()
            .With(c => c.Id, _creditId)
            .With(c => c.ProjectId, _projectId)
            .With(c => c.IssuedAt, _issuedAt)
            .With(c => c.RetiredAt, (DateTime?)null)
            .Create();

        var account = (await _fileStore.GetAsync<Account>($"{_basePath}/accounts/{_accountId}", CancellationToken.None)).Unwrap();
        var updatedAccount = new Account
        {
            Id = account.Id,
            Name = account.Name,
            CreatedAt = account.CreatedAt,
            Credits = [credit]
        };

        await _fileStore.SaveAsync($"{_basePath}/accounts/{_accountId}", updatedAccount, CancellationToken.None);
    }

    private async Task A_Transfer_Credit_Request_Is_Sent(Guid accountId, Guid creditId, Guid recipientAccountId)
    {
        _scopes[AccountId] = accountId.ToString();
        _scopes[CreditId] = creditId.ToString();
        _scopes[RecipientAccountId] = recipientAccountId.ToString();
        _httpResponse = await _client.TransferCredit(accountId, creditId, new TransferCreditRequest
        {
            RecipientAccountId = recipientAccountId
        });
    }

    private async Task The_Sender_Should_No_Longer_Have_The_Credit()
    {
        var account = (await _fileStore.GetAsync<Account>($"{_basePath}/accounts/{_accountId}", CancellationToken.None)).Unwrap();

        account.Credits.Should().NotContain(c => c.Id == _creditId);
    }

    private async Task The_Recipient_Should_Now_Have_The_Credit()
    {
        var account = (await _fileStore.GetAsync<Account>($"{_basePath}/accounts/{_recipientAccountId}", CancellationToken.None)).Unwrap();

        account.Credits.Should().Contain(c => c.Id == _creditId);
    }
}
