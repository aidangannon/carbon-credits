using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Host.Models;

namespace Acceptance.Infrastructure.Extensions;

public static class ProjectResponseAssertions
{
    public static void ShouldEqual(this ProjectResponse response, CreateProjectRequest request)
    {
        using var scope = new AssertionScope();
        response.Id.Should().NotBe(Guid.Empty);
        response.Name.Should().Be(request.Name);
        response.Country.Should().Be(request.Country);
        response.Type.Should().Be(request.Type);
    }
}
