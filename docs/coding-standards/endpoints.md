[← Back to Index](./index.md)

# Coding Standards: Endpoints

> Each endpoint is a single static class in its own file. A per-feature mapper wires it into a route group. A root mapper wires all feature mappers into the app. Everything is explicit at the endpoint level.

## Structure

Each endpoint lives in one file under `src/Host/Handlers/Endpoints/<Feature>/`:

| File | Purpose |
|---|---|
| `<Action>Endpoint.cs` | The handler - one public extension method on `RouteGroupBuilder`, one private static handler method |
| `<Feature>EndpointMapper.cs` | Creates the route group and chains all endpoint extension methods for that feature |
| `EndpointMapper.cs` | Root mapper - calls every feature mapper and returns the app |

See [`CreateAccountEndpoint.cs`](/src/Host/Handlers/Endpoints/Accounts/CreateAccountEndpoint.cs), [`AccountsEndpointMapper.cs`](/src/Host/Handlers/Endpoints/Accounts/AccountsEndpointMapper.cs), and [`EndpointMapper.cs`](/src/Host/Handlers/Endpoints/EndpointMapper.cs).

## Handler File

One public extension method registers the route. One private static method is the handler. Nothing else lives in the file:

```csharp
public static class CreateAccountEndpoint
{
    public static RouteGroupBuilder MapCreateAccount(this RouteGroupBuilder group)
    {
        group
            .MapPost("", CreateAccount)
            .WithSummary("Creates an account")
            .WithDescription("Creates a new account with the given name");

        return group;
    }

    private static async Task<Results<Created<AccountResponse>, ValidationProblem, ProblemHttpResult>> CreateAccount(
        [FromBody] CreateAccountRequest request,
        [FromServices] IValidator<CreateAccountRequest> validator,
        [FromServices] IAccountCreationService accountCreationService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    { ... }
}
```

## Dependency Injection

Inject all dependencies via `[FromServices]` attributes on the handler method parameters. Do not use constructor injection - handlers are static. This keeps dependencies explicit and co-located with the handler that uses them.

## Feature Mapper

Each feature has a mapper that creates the group and chains all endpoints for that feature:

```csharp
public static class AccountsEndpointMapper
{
    public static RouteGroupBuilder MapAccountEndpoints(this WebApplication app)
    {
        return app
            .MapGroup("accounts")
            .MapGetAccountById()
            .MapCreateAccount()
            .WithTags("Accounts");
    }
}
```

## Root Mapper

[`EndpointMapper.cs`](/src/Host/Handlers/Endpoints/EndpointMapper.cs) is the single place that calls every feature mapper. Adding a new feature means adding one line here:

```csharp
public static class EndpointMapper
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapAccountEndpoints();
        app.MapProjectEndpoints();

        return app;
    }
}
```

## Auth and Filters

Apply auth and filters at the endpoint level using extension methods - not globally, not in middleware. This makes the security posture of each endpoint explicit and visible in its file:

```csharp
group
    .MapPost("", CreateAccount)
    .RequireAuthorization()
    .AddEndpointFilter<MyFilter>();
```

Never rely on a global auth policy to cover an endpoint silently. If an endpoint requires auth, it must say so explicitly.
