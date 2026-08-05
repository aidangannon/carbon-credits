[← Back to Index](./index.md)

# Coding Standards: Core Values

> These are the principles the codebase is built around. When in doubt about a design decision, come back here.

## Explicit Over Implicit

If it matters, it must be visible. Never rely on convention, global config, middleware, or inherited behaviour to do something important silently.

Code should state its intent directly. Auth, filters, dependencies, logging scope - all of it should be readable in the file where it happens. If a reader has to know what some global registration is doing in the background to understand a piece of code, that code is hiding something.

Middleware is acceptable for genuinely crosscutting concerns (e.g. global error handling, request timing). It is not acceptable as a place to bury logic that is specific to certain endpoints or flows.

## Vertical Slicing

Organise code by feature, not by layer. Each feature is a vertical slice from endpoint to persistence - its own handler, its own service, its own test. Slices should be independently readable and independently changeable without coupling to other slices.

Duplication across slices is acceptable. Coupling between slices is not. DRY is secondary to isolation.

## Invariants Belong on the Model

Business rules and state transitions live on the aggregate or model, not in the service. Services are a script: load, act, save. The aggregate owns the invariants so they are testable without infrastructure and enforced everywhere they are used, not just in one service.

## Test First

Write acceptance tests before writing implementation. Acceptance tests are the living gospel of how the service behaves - they document exactly what the API does, including all edge cases, in plain language.

The architecture should make this natural. If a feature is hard to acceptance-test, that is a signal the design is wrong, not that the test should be skipped.

## Self-Documenting Architecture

The structure of the codebase should explain itself. Where code lives, how it is named, and how it is connected should tell the reader what the system does without needing a guide. The docs exist to explain the rules - the code should demonstrate them.
