# Architecture: Ports & Adapters

> **Guide:** Use the table to place code. Read the example files before creating anything new.

---

## Placement

| Code type | Path | Example |
|---|---|---|
| Business logic / state mutation | `src/Core/Models/<Aggregate>.cs` : method on the aggregate | `src/Core/Models/Account.cs` |
| Domain error | `src/Core/Errors/<Aggregate>Errors.cs` | `src/Core/Errors/AccountErrors.cs` |
| **I/O interface** (DB, file, API, hardware) | `src/Application/Ports/I<Name>.cs` | `src/Application/Ports/IAccountRepository.cs` |
| **I/O adapter** (implements the port) | `src/Persistence/Adapters/<Name>.cs` | `src/Persistence/Adapters/FileAccountRepository.cs` |
| Use-case service | `src/Application/Slices/<Feature>/<Name>Service.cs` | `src/Application/Slices/Accounts/AccountCreationService.cs` |
| HTTP endpoint | `src/Host/Handlers/Endpoints/<Feature>/` | `src/Host/Handlers/Endpoints/Accounts/CreateAccountEndpoint.cs` |
| DI registration | `src/Host/Ioc/DependencyExtensions.cs` | : |
| Shared primitive | `src/Crosscutting/` | `src/Crosscutting/Result/Result.cs` |


---

## Why

- **Ports (`Application/Ports/I*.cs`) are the only interfaces that matter architecturally** : they exist because the adapter must be swappable (e.g. swap file for a real DB without touching any other layer)
- **Slice service interfaces** (e.g. `IAccountCreationService`) co-locate with their implementation; they exist purely to make unit testing possible via mocking : not for swappability
- **Logic on the aggregate, not the service** : services are a script (load → act → save); the aggregate owns invariants so they're testable without infrastructure
- **`Core` has zero outward dependencies** : if it referenced `Persistence` or `Host`, the domain would couple to infrastructure details

---

## Dependency Order

```
Host → Application (Slices + Ports) → Core
          ↑
     Persistence (implements Ports)
     Crosscutting (used by all)
```
