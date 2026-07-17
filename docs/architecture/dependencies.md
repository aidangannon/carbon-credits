# Architecture: Dependencies

> **Guide:** How packages are managed and where to add new dependencies.

---

## Project References

The exact set of projects is flexible : new modules can be added freely. The hard rules are:

| Layer | Rule |
|---|---|
| `Core` | **No dependencies on anything.** Contains only business rules, behaviour, and schemas. |
| `Application` | **Depends only on `Core`.** Ports (I/O interfaces) live inside `Application` : they are internal to the module and registered against their adapters by `Host` at runtime. |
| `Host` | **Can depend on anything.** It is the composition root : it wires everything together at runtime. |

Each project's actual references are declared in its `.csproj` : e.g. [`Host.csproj`](/src/Host/Host.csproj), [`Application.csproj`](/src/Application/Application.csproj), [`Persistence.csproj`](/src/Persistence/Persistence.csproj).

---

## Central Package Management (CPM)

All NuGet package **versions** are declared once in [`Directory.Packages.props`](/Directory.Packages.props) at the repo root. Individual `.csproj` files reference packages **without a version** : the version is resolved centrally. See [`Host.csproj`](/src/Host/Host.csproj) for an example of a project referencing packages, and [`Directory.Packages.props`](/Directory.Packages.props) for the corresponding version declarations.

This prevents version drift across projects and makes upgrades a one-line change in a single file.

---

## Adding a New Package

`dotnet add package` handles everything automatically : `cd` into the relevant project directory and run it against the package name. It will:
1. Add the version entry to [`Directory.Packages.props`](/Directory.Packages.props)
2. Add the versionless reference to the `.csproj` in the current directory

No manual edits needed. The same workflow applies to any project in the solution.

---
