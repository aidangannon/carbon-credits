using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

public static class Http
{
    public static void The_Response_Should_Have_Status_Code_STATUS_CODE(
        HttpStatusCode statusCode,
        HttpResponseMessage response
    )
    {
        response.StatusCode.Should().Be(statusCode);
    }

    public static void The_Response_Should_Not_Have_Status_Code_STATUS_CODE(
        HttpStatusCode statusCode,
        HttpResponseMessage response
    )
    {
        response.StatusCode.Should().NotBe(statusCode);
    }

    public static async Task The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(
        HttpStatusCode status,
        string detail,
        HttpResponseMessage response
    )
    {
        response.StatusCode.Should().Be(status);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be((int)status);
        problem.Detail.Should().Contain(detail);
    }
}