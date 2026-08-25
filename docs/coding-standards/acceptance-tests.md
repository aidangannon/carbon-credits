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

## Parameterised Step Names

When a step method takes a parameter, suffix the method name with the parameter's name in uppercase (e.g. `A_Recipient_Account_Exists_Created_At_CREATED(DateTime createdAt)`). LightBDD matches the uppercase segment of the method name against the argument passed at the call site and renders it in the readable scenario output, so the value shows up inline instead of being hidden. See [`HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE`](/tests/Acceptance/CommonSteps/HttpSteps.cs) for the established pattern.

## Common Steps

[`CommonSteps/`](/tests/Acceptance/CommonSteps/) contains static steps for crosscutting concerns only, such as asserting HTTP status codes or checking log output. See [`HttpSteps.cs`](/tests/Acceptance/CommonSteps/HttpSteps.cs) and [`LogSteps.cs`](/tests/Acceptance/CommonSteps/LogSteps.cs).

Nothing business, domain, or use-case specific belongs in common steps. If a step is specific to an entity or scenario, it belongs in the relevant `.steps.cs`.

## Infrastructure Extensions

[`Infrastructure/Extensions/`](/tests/Acceptance/Infrastructure/Extensions/) hides the boilerplate of hitting endpoints behind typed extension methods on `HttpClient`. Each entity gets its own extensions file - see [`AccountClientExtensions.cs`](/tests/Acceptance/Infrastructure/Extensions/AccountClientExtensions.cs) and [`ProjectClientExtensions.cs`](/tests/Acceptance/Infrastructure/Extensions/ProjectClientExtensions.cs).

The same pattern can be applied to the persistence layer if needed.

The rule for what goes here: infrastructure and boilerplate reduction only. No business logic, no assertions, no domain knowledge. Only pull something into infrastructure when there is enough repetition across `.steps.cs` files to warrant it - do not pre-emptively extract.

## Summary of Rules

| Rule | Reason |
|---|---|
| Each slice owns its steps | Keeps slices open-closed and independently changeable |
| Common steps contain no domain logic | Prevents crosscutting files from accumulating business knowledge |
| Infrastructure hides boilerplate only | Keeps `.steps.cs` files readable without leaking concerns upward |
| Extract to infrastructure only when repetition warrants it | Avoids premature abstraction |
