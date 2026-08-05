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

One public extension method registers the route. One private static method is the handler. Nothing else lives in the file. See [`CreateAccountEndpoint.cs`](/src/Host/Handlers/Endpoints/Accounts/CreateAccountEndpoint.cs) and [`GetAccountByIdEndpoint.cs`](/src/Host/Handlers/Endpoints/Accounts/GetAccountByIdEndpoint.cs).

## Dependency Injection

Inject all dependencies via `[FromServices]` attributes on the handler method parameters. Do not use constructor injection - handlers are static. This keeps dependencies explicit and co-located with the handler that uses them.

## Feature Mapper

Each feature has a mapper that creates the route group and chains all its endpoints. See [`AccountsEndpointMapper.cs`](/src/Host/Handlers/Endpoints/Accounts/AccountsEndpointMapper.cs) and [`ProjectsEndpointMapper.cs`](/src/Host/Handlers/Endpoints/Projects/ProjectsEndpointMapper.cs).

## Root Mapper

[`EndpointMapper.cs`](/src/Host/Handlers/Endpoints/EndpointMapper.cs) is the single place that calls every feature mapper. Adding a new feature means adding one line here.

## Auth and Filters

Apply auth and filters at the endpoint level using extension methods - not globally, not in middleware. This makes the security posture of each endpoint explicit and visible in its file. Use `.RequireAuthorization()` and `.AddEndpointFilter<T>()` chained on the route registration.

Never rely on a global auth policy to cover an endpoint silently. If an endpoint requires auth, it must say so explicitly.
