using Host.Handlers.Endpoints.Accounts;

namespace Host.Handlers.Endpoints;

public static class EndpointMapper
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapAccountEndpoints();

        return app;
    }
}
