namespace Acceptance.Infrastructure.Extensions;

public static class AccountClientExtensions
{
    public static async Task<HttpResponseMessage> GetAccountById(this HttpClient client, Guid id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get
        };

        return await client.SendAsync();
    }
}
