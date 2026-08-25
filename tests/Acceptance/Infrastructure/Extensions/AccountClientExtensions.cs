using System.Net.Http.Json;
using Host.Models;

namespace Acceptance.Infrastructure.Extensions;

public static class AccountClientExtensions
{
    public static async Task<HttpResponseMessage> GetAccountById(this HttpClient client, Guid id, bool? includeRetiredCredits = null, bool? includeFutureCredits = null)
    {
        var query = new List<string>();
        if (includeRetiredCredits is not null)
        {
            query.Add($"includeRetiredCredits={includeRetiredCredits}");
        }

        if (includeFutureCredits is not null)
        {
            query.Add($"includeFutureCredits={includeFutureCredits}");
        }

        var queryString = query.Count > 0 ? $"?{string.Join("&", query)}" : string.Empty;

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"/accounts/{id}{queryString}", UriKind.Relative),
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
}

