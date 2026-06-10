namespace Host.Handlers.Endpoints;

public static class EndpointMapper
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapEndpoints();

        return app;
    }
}
