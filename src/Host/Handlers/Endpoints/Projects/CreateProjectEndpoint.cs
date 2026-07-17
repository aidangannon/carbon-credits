using Application.Slices.Projects;
using FluentValidation;
using Host.Constants;
using Host.Extensions;
using Host.Mappers.Projects;
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

    private static async Task<Results<Created<ProjectResponse>, ValidationProblem, ProblemHttpResult>> CreateProject(
        [FromBody] CreateProjectRequest request,
        [FromServices] IValidator<CreateProjectRequest> validator,
        [FromServices] IProjectCreationService projectCreationService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        var logger = loggerFactory.CreateLogger(LoggingOperations.CreateProject);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            [Operation] = LoggingOperations.CreateProject,
            [ProjectName] = request.Name
        });

        logger.LogInformation("Endpoint Called");

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var project = request.ToProject();
        var serviceResult = await projectCreationService.CreateProject(project, cancellationToken);

        logger.LogInformation("Endpoint Completed");

        if (serviceResult.HasFailed())
        {
            return serviceResult.ToProblemResult();
        }

        var projectResponse = serviceResult.Unwrap().ToResponse();

        return TypedResults.Created($"/projects/{projectResponse.Id}", projectResponse);
    }
}
