using Host.Handlers.Endpoints.Accounts;
using Host.Handlers.Endpoints.Projects;

namespace Host.Handlers.Endpoints;

public static class EndpointMapper
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapAccountEndpoints();
        app.MapProjectEndpoints();

        return app;
    }
}
