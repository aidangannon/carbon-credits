[← Back to Index](./index.md)

# Coding Standards: Unit Tests

> Unit tests are not the primary way this codebase is tested. Acceptance tests are. Unit tests exist for one purpose: covering edge cases in pure logic that acceptance tests cannot practically reach.

## When to Write a Unit Test

Write a unit test when the code under test is a pure function with multiple distinct cases that are impractical to exercise end-to-end. The clearest examples are:

- **Mappers** - every field mapping should be asserted, and edge cases like empty collections tested explicitly. See [`AccountMapperTests.cs`](/tests/Unit/Host/Mappers/AccountMapperTests.cs) and [`CreditMapperTests.cs`](/tests/Unit/Host/Mappers/CreditMapperTests.cs).
- **Validators** - each validation rule (empty, whitespace, out of range, etc.) is a unit test. Acceptance tests cover the happy path; unit tests cover the invalid cases. See [`CreateAccountRequestValidatorTests.cs`](/tests/Unit/Host/Validators/CreateAccountRequestValidatorTests.cs).
- **Extensions and utilities** - shared primitives like `Result` have behaviour that needs direct verification. See [`ResultTests.cs`](/tests/Unit/Crosscutting/ResultTests.cs).

## When Not to Write a Unit Test

Do not unit test:

- Handlers or services. Their behaviour is covered by acceptance tests, which test the whole vertical slice end-to-end. A unit test on a service means mocking its dependencies - that test is verifying wiring, not behaviour.
- Domain models in isolation unless they contain meaningful logic with edge cases acceptance tests cannot reach.
- Anything that would require a mock of a non-trivial dependency. If you need a mock to write the test, the behaviour should be covered at the acceptance level instead.

## Structure

Mirror the source structure under `tests/Unit/`. One test class per source class, suffixed with `Tests`. Use `AssertionScope` when asserting multiple fields so all failures are reported together, not just the first. See [`AccountMapperTests.cs`](/tests/Unit/Host/Mappers/AccountMapperTests.cs) for an example of both conventions.

## Summary

Unit tests fill the gaps acceptance tests leave - edge cases in pure logic. They do not replace acceptance tests and they do not test behaviour that spans layers. If a test needs mocks or infrastructure, it is in the wrong place.
