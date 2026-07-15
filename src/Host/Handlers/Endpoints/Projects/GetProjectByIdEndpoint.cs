using Application.Slces.Projects;
using Host.Constants;
using Host.Extensions;
using Host.Mappers.Projects;
using Host.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Host.Handlers.Endpoints.Projects;

public static class GetProjectByIdEndpoint
{
    public static RouteGroupBuilder MapGetProjectById(this RouteGroupBuilder group)
    {
        group
            .MapGet("{id:guid}", GetProjectById)
            .WithSummary("Gets project by id")
            .WithDescription("Returns a project by id, or 404 if not found");

        return group;
    }

    private static async Task<Results<Ok<ProjectResponse>, ProblemHttpResult>> GetProjectById(
        [FromRoute] Guid id,
        [FromServices] IProjectRetrievalService projectRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        var logger = loggerFactory.CreateLogger(LoggingOperations.GetProjectById);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            [Operation] = LoggingOperations.GetProjectById,
            [ProjectId] = id.ToString()
        });

        logger.LogInformation("Endpoint Called");

        var serviceResult = await projectRetrievalService.GetProjectById(id, cancellationToken);

        logger.LogInformation("Endpoint Completed");

        if (serviceResult.HasFailed())
        {
            return serviceResult.ToProblemResult();
        }

        return TypedResults.Ok(serviceResult.Unwrap().ToResponse());
    }
}
