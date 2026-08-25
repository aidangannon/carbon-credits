[← Back to Index](../index.md)

# Coding Standards: Index

| Document | Scope |
|---|---|
| [Core Values](./core-values.md) | The principles the codebase is built around: explicit over implicit, vertical slicing, invariants on the model, test first, self-documenting architecture |
| [Endpoints](./endpoints.md) | Single file per handler, feature mappers, root mapper, `[FromServices]` injection, explicit auth and filters |
| [Logging](./logging.md) | Bind logger to the operation, scope all request data, `LoggingKeys` and `LoggingOperations` constants |
| [Unit Tests](./unit-tests.md) | When to unit test (mappers, validators, extensions), when not to, and how to structure the test project |
| [Acceptance Tests](./acceptance-tests.md) | Structure of feature files and steps, step ownership, when to use common steps vs infrastructure extensions, verifying mutations against both the response and persisted state, and what belongs where |
