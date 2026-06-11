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
}
