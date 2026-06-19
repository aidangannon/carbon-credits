using Host.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Host.Handlers.Endpoints.Projects;

public static class CreateProjectEndpoint
{
    public static RouteGroupBuilder MapCreateProject(this RouteGroupBuilder group)
    {
        group
            .MapPost("", CreateProject)
            .WithSummary("Creates a project")
            .WithDescription("Creates a new carbon credit project");

        return group;
    }

    private static Task<Results<Created<ProjectResponse>, ValidationProblem, ProblemHttpResult>> CreateProject(
        [FromBody] CreateProjectRequest request,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        Results<Created<ProjectResponse>, ValidationProblem, ProblemHttpResult> result =
            TypedResults.Created("/projects/stub", new ProjectResponse
            {
                Id = Guid.Empty,
                Name = string.Empty,
                Country = string.Empty,
                Type = string.Empty
            });

        return Task.FromResult(result);
    }
}
