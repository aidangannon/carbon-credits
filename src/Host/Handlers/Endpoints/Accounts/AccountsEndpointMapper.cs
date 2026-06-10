namespace Host.Handlers.Endpoints.Accounts;

public static class AccountsEndpointMapper
{
    public static RouteGroupBuilder MapUserEndpoints(this WebApplication app)
    {
        return app
            .MapGroup("accounts")
            .MapGetAccountById()
            .WithTags("Accounts");
    }
}
