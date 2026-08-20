[← Back to Index](./index.md)

# Architecture: Ports & Adapters

> **Guide:** Use the table to place code. Read the example files before creating anything new.

## Placement

| Code type | Path | Example |
|---|---|---|
| Business logic / state mutation | `src/Core/Models/<Aggregate>.cs` - method on the aggregate | [`src/Core/Models/Account.cs`](/src/Core/Models/Account.cs) |
| Domain error | `src/Core/Errors/<Aggregate>Errors.cs` | [`src/Core/Errors/AccountErrors.cs`](/src/Core/Errors/AccountErrors.cs) |
| I/O interface (DB, file, API, hardware) | `src/Application/Ports/I<Name>.cs` | [`src/Application/Ports/IAccountRepository.cs`](/src/Application/Ports/IAccountRepository.cs) |
| I/O adapter (implements the port) | `src/Persistence/Adapters/<Name>.cs` | [`src/Persistence/Adapters/FileAccountRepository.cs`](/src/Persistence/Adapters/FileAccountRepository.cs) |
| Generic storage mechanics (load/save, locking, versioning) | `src/Persistence/<Store>.cs` | [`src/Persistence/FileStore.cs`](/src/Persistence/FileStore.cs) |
| Use-case service | `src/Application/Slices/<Feature>/<Name>Service.cs` | [`src/Application/Slices/Accounts/AccountCreationService.cs`](/src/Application/Slices/Accounts/AccountCreationService.cs) |
| HTTP endpoint | `src/Host/Handlers/Endpoints/<Feature>/` | [`src/Host/Handlers/Endpoints/Accounts/CreateAccountEndpoint.cs`](/src/Host/Handlers/Endpoints/Accounts/CreateAccountEndpoint.cs) |
| DI registration | `src/Host/Ioc/DependencyExtensions.cs` | [`src/Host/Ioc/DependencyExtensions.cs`](/src/Host/Ioc/DependencyExtensions.cs) |
| Shared primitive | `src/Crosscutting/` | [`src/Crosscutting/Result/Result.cs`](/src/Crosscutting/Result/Result.cs) |

## Why

- Ports (`Application/Ports/I*.cs`) are the only interfaces that matter architecturally - they exist because the adapter must be swappable (e.g. swap file for a real DB without touching any other layer)
- Slice service interfaces (e.g. `IAccountCreationService`) co-locate with their implementation; they exist purely to make unit testing possible via mocking, not for swappability
- Logic on the aggregate, not the service - services are a script (load → act → save); the aggregate owns invariants so they're testable without infrastructure
- `Core` has zero outward dependencies - if it referenced `Persistence` or `Host`, the domain would couple to infrastructure details

## Retrieval Is Generic, Invariants Are Not

Repositories (e.g. [`FileAccountRepository`](/src/Persistence/Adapters/FileAccountRepository.cs)) only do two things: build the key for an aggregate and translate a storage outcome into a domain error (`AccountErrors.NotFound`, etc.). They never contain locking, serialization, versioning, or query logic - that mechanics is generic and lives once, in a shared store (e.g. [`FileStore`](/src/Persistence/FileStore.cs)), reused by every repository regardless of which aggregate it serves.

This works because every repository does the same two things - `GetByIdAsync` and `SaveAsync` for one whole aggregate by id. There is no partial update, no filtering, no joining across aggregates. An aggregate is always loaded whole and saved whole:

- **Load** fetches the full aggregate and its current version (e.g. an etag) in one step.
- **Act** happens entirely on the aggregate (e.g. `Account.AddCredit`) - this is where every invariant is enforced. The repository and store never see or validate business rules.
- **Save** writes the full aggregate back, and the store rejects the write if the version has moved on since it was loaded (optimistic concurrency, scoped per-aggregate/per-partition so unrelated aggregates never contend with each other).

If a use case seems to need a bespoke query, a partial update, or reaching across aggregates, that's a signal the invariant belongs on the model, or the aggregate boundary is drawn wrong - not that the repository or store should grow query capability. Keep data access as generic and boring as possible; keep all the interesting rules on the model.

The one legitimate exception is a large nested collection on an aggregate (e.g. an account with thousands of credits) - here the safe default is pagination on read, not a bespoke query, since the aggregate is still loaded/saved as a whole and only the response shape is windowed. If a single partition is seeing heavy collision/contention under concurrent writes, that is itself a signal the aggregate boundary is too coarse and should be reconsidered first. Bespoke, one-off querying is only acceptable where re-drawing the boundary isn't feasible and generic whole-aggregate load/save would otherwise cause real performance degradation.

## Dependency Order

```
Host → Application (Slices + Ports) → Core
          ↑
     Persistence (implements Ports)
     Crosscutting (used by all)
```
