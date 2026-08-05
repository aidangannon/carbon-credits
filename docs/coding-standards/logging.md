[← Back to Index](./index.md)

# Coding Standards: Logging

> Logging must be bound to the action or handler - never use a generic category. Scope everything you know from the request so logs are self-contained and debuggable without cross-referencing other log lines.

## Keys and Operations

All log keys are constants in [`LoggingKeys.cs`](/src/Host/Constants/LoggingKeys.cs). All operation names are constants in [`LoggingOperations.cs`](/src/Host/Constants/LoggingOperations.cs).

- Add a new key to `LoggingKeys` for every piece of request data you want to appear in structured logs (e.g. `AccountId`, `ProjectName`)
- Add a new operation to `LoggingOperations` for every handler or action

Never use raw strings for keys or operation names inline.

## Binding the Logger to the Operation

Create the logger from `ILoggerFactory` using the operation name as the category. Every log line from that handler is then tagged with the operation, making it trivially filterable.

Inject `ILoggerFactory` via `[FromServices]` like any other dependency - do not inject `ILogger<T>` in handlers.

## Scoping

Open a scope immediately after creating the logger. Put everything you know from the request into the scope so it appears on every log line for the lifetime of the handler.

Add more keys to the scope as they become available - for example, after a service call resolves an ID, add it to a new nested scope before logging completion.

Scope as much as possible. The goal is that any single log line contains enough context to understand what was happening without needing surrounding lines.

## What to Log

Log at the boundary of the handler - called and completed. Log everything from the request that is useful for debugging. If the request has an ID, name, or any discriminating field, it belongs in the scope.

Do not log inside services or the domain - logging belongs at the handler boundary. Services and domain logic should remain unaware of logging.

## Examples

See [`CreateAccountEndpoint.cs`](/src/Host/Handlers/Endpoints/Accounts/CreateAccountEndpoint.cs) and [`GetAccountByIdEndpoint.cs`](/src/Host/Handlers/Endpoints/Accounts/GetAccountByIdEndpoint.cs).
