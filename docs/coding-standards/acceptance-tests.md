[← Back to Index](./index.md)

# Coding Standards: Acceptance Tests

> These tests are the living documentation of the API. They document exactly how the API behaves, including all edge cases.

## Structure

Each feature is a vertical slice with two files that work as a pair:

| File | Purpose |
|---|---|
| `Feature.cs` | The readable index: scenario names only, written in plain language. This is what anyone reads to understand what the feature does. |
| `Feature.steps.cs` | The step definitions for that feature. Contains all the setup, execution, and assertions for those scenarios. |

See [`Create_Account.cs`](/tests/Acceptance/Features/Accounts/Create_Account.cs) and [`Create_Account.steps.cs`](/tests/Acceptance/Features/Accounts/Create_Account.steps.cs) as an example pair.

## Step Ownership

Each `.steps.cs` file should own its own steps. Steps that retrieve or manipulate the same entity may be duplicated across slices - this is intentional. It keeps each slice open-closed: both can change independently without being coupled to each other. DRY is secondary to isolation here.

## Common Steps

[`CommonSteps/`](/tests/Acceptance/CommonSteps/) contains static steps for crosscutting concerns only, such as asserting HTTP status codes or checking log output. See [`HttpSteps.cs`](/tests/Acceptance/CommonSteps/HttpSteps.cs) and [`LogSteps.cs`](/tests/Acceptance/CommonSteps/LogSteps.cs).

Nothing business, domain, or use-case specific belongs in common steps. If a step is specific to an entity or scenario, it belongs in the relevant `.steps.cs`.

## Infrastructure Extensions

[`Infrastructure/Extensions/`](/tests/Acceptance/Infrastructure/Extensions/) hides the boilerplate of hitting endpoints behind typed extension methods on `HttpClient`. Each entity gets its own extensions file - see [`AccountClientExtensions.cs`](/tests/Acceptance/Infrastructure/Extensions/AccountClientExtensions.cs) and [`ProjectClientExtensions.cs`](/tests/Acceptance/Infrastructure/Extensions/ProjectClientExtensions.cs).

The same pattern can be applied to the persistence layer if needed.

The rule for what goes here: infrastructure and boilerplate reduction only. No business logic, no assertions, no domain knowledge. Only pull something into infrastructure when there is enough repetition across `.steps.cs` files to warrant it - do not pre-emptively extract.

## Verifying Mutations

For any scenario that adds, updates, or deletes state (e.g. creating a credit, retiring a credit), assert both sides of the mutation:

- **The response** - the immediate HTTP result (status code, and body if one is returned).
- **The underlying record(s)** - re-fetch the affected aggregate (via its retrieval endpoint, or directly from the store) and assert the persisted state actually reflects the change.

Asserting only the response can pass even if the write never happened or was applied incorrectly. See [`Retire_Credit.steps.cs`](/tests/Acceptance/Features/Accounts/Retire_Credit.steps.cs) - the happy-path scenario asserts the `204` response and then re-fetches the account via `GetAccountById` to confirm the credit is actually retired.

## Summary of Rules

| Rule | Reason |
|---|---|
| Each slice owns its steps | Keeps slices open-closed and independently changeable |
| Common steps contain no domain logic | Prevents crosscutting files from accumulating business knowledge |
| Infrastructure hides boilerplate only | Keeps `.steps.cs` files readable without leaking concerns upward |
| Extract to infrastructure only when repetition warrants it | Avoids premature abstraction |
| Mutating scenarios assert the response and the persisted record | A correct-looking response doesn't guarantee the write was applied |
