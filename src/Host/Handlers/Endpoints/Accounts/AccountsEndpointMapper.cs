namespace Host.Handlers.Endpoints.Accounts;

public static class AccountsEndpointMapper
{
    public static RouteGroupBuilder MapAccountEndpoints(this WebApplication app)
    {
        return app
            .MapGroup("accounts")
            .MapGetAccountById()
            .WithTags("Accounts");
    }
}
