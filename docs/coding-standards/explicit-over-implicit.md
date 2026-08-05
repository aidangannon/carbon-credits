[← Back to Index](./index.md)

# Coding Standards: Explicit Over Implicit

> If it matters, it must be visible. Never rely on convention, global config, or inherited behaviour to do something important silently.

## The Rule

Prefer code that states its intent directly over code that relies on something elsewhere to infer it. The reader should be able to understand what a piece of code does without needing to know what framework defaults, global registrations, or base classes are doing behind it.

## In Practice

**Auth** - every endpoint that requires authorization must call `.RequireAuthorization()` (or the relevant policy overload) on that endpoint. A global fallback policy is not sufficient. If someone reads the endpoint file, they must be able to see whether it is protected.

**Filters** - apply filters with `.AddEndpointFilter<T>()` at the endpoint or group level. Do not register endpoint-specific behaviour globally and rely on it being applied everywhere.

**Dependencies** - inject via `[FromServices]` on the handler method. The handler's dependencies are visible at its signature, not hidden in a constructor somewhere else.

**Logging scope** - put every piece of relevant request data into the log scope explicitly. Do not assume the framework or middleware will carry context through. See [Logging](./logging.md).

**Naming** - use `LoggingKeys` and `LoggingOperations` constants rather than inline strings. The name of the operation and the keys in structured logs are explicit contracts, not magic strings scattered across files.

## Why

Implicit behaviour is invisible behaviour. It fails silently, it surprises new contributors, and it makes auditing (security, correctness, observability) much harder. Explicit code is more verbose but it is honest - it tells the reader exactly what it does.

When something is explicit, it can be reviewed, tested, and changed with confidence. When it is implicit, a change somewhere else can break it without any warning.
