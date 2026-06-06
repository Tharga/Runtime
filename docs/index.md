---
_layout: landing
---

# Tharga.Runtime

Assembly scanning, runtime type discovery, and filter-based dependency injection for **.NET 8 / 9 / 10**. Provides `IAssemblyService` (cached type lookups across loaded assemblies), filter-based `IServiceCollection` extensions for registering many implementations of an interface in one call, and a small set of `Type` helpers used across Tharga libraries.

## Package

| Package | What it does |
|---|---|
| [Tharga.Runtime](https://www.nuget.org/packages/Tharga.Runtime) | Assembly/type scanning, cached `GetTypes` lookups, and filter-based DI registration. |

## Quick start

```
dotnet add package Tharga.Runtime
```

Register the cache-backed assembly service:

```csharp
using Tharga.Runtime;

builder.Services.AddAssemblyService();
```

Or skip `IAssemblyService` and register every implementation of a base type in one line:

```csharp
builder.Services.AddSingleton<IMyHandler>(x => !x.IsAbstract);
```

## What's in the box

- **`IAssemblyService`** — cached, filterable type discovery across loaded assemblies. See [Assembly service](articles/assembly-service.md).
- **`IServiceCollection` filter extensions** — `Add`, `AddTransient`, `AddScoped`, `AddSingleton` overloads that scan and register types matching a `TypeInfo` filter. See [Service registration](articles/service-registration.md).
- **`Type` helpers** — `IsOfType`, `GetDirectlyImplementedInterfaces`, `ToAssemblyQualifiedNameWithoutVersion`, and version-tolerant `GetType(string)`. See [Type extensions](articles/type-extensions.md).

## Repo

[github.com/Tharga/Runtime](https://github.com/Tharga/Runtime) — source, issues, releases.
