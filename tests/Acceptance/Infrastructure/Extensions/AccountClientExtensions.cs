using System.Net.Http.Json;
using Host.Models;

namespace Acceptance.Infrastructure.Extensions;

public static class AccountClientExtensions
{
    public static async Task<HttpResponseMessage> GetAccountById(this HttpClient client, Guid id)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"/accounts/{id}"),
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
}

