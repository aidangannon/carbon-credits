namespace Host.Handlers.Endpoints.Projects;

public static class ProjectsEndpointMapper
{
    public static RouteGroupBuilder MapProjectEndpoints(this WebApplication app)
    {
        return app
            .MapGroup("projects")
            .MapCreateProject()
            .MapGetProjectById()
            .WithTags("Projects");
    }
}
