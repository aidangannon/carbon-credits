using System.Net.Http.Json;
using Host.Models;

namespace Acceptance.Infrastructure.Extensions;

public static class ProjectClientExtensions
{
    public static async Task<HttpResponseMessage> CreateProject(this HttpClient client, CreateProjectRequest body)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("/projects"),
            Method = HttpMethod.Post,
            Content = JsonContent.Create(body)
        };

        return await client.SendAsync(request);
    }
}
