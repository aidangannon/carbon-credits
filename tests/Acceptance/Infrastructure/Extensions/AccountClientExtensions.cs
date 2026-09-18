using System.Net.Http.Json;
using Host.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace Acceptance.Infrastructure.Extensions;

public static class AccountClientExtensions
{
    public static async Task<HttpResponseMessage> GetAccountById(this HttpClient client, Guid id, bool? includeRetiredCredits = null, bool? includeFutureCredits = null)
    {
        var query = new Dictionary<string, string?>();
        if (includeRetiredCredits is not null)
        {
            query["includeRetiredCredits"] = includeRetiredCredits.ToString();
        }

        if (includeFutureCredits is not null)
        {
            query["includeFutureCredits"] = includeFutureCredits.ToString();
        }

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(QueryHelpers.AddQueryString($"/accounts/{id}", query), UriKind.Relative),
            Method = HttpMethod.Get
        };

        return await client.SendAsync(request);
    }

    public static async Task<HttpResponseMessage> CreateAccount(this HttpClient client, CreateAccountRequest body)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("/accounts"),
            Method = HttpMethod.Post,
            Content = JsonContent.Create(body)
        };

        return await client.SendAsync(request);
    }

    public static async Task<HttpResponseMessage> CreateCredit(this HttpClient client, Guid accountId, Guid projectId, CreateCreditRequest body)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"/accounts/{accountId}/credits"),
            Method = HttpMethod.Post,
            Content = JsonContent.Create(body)
        };

        return await client.SendAsync(request);
    }

    public static async Task<HttpResponseMessage> TransferCredit(this HttpClient client, Guid accountId, Guid creditId, TransferCreditRequest body)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"/accounts/{accountId}/credits/{creditId}/transfer"),
            Method = HttpMethod.Post,
            Content = JsonContent.Create(body)
        };

        return await client.SendAsync(request);
    }

    public static async Task<HttpResponseMessage> RetireCredit(this HttpClient client, Guid accountId, Guid creditId)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"/accounts/{accountId}/credits/{creditId}"),
            Method = HttpMethod.Delete
        };

        return await client.SendAsync(request);
    }
}

