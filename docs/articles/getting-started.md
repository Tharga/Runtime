# Getting started

`Tharga.Runtime` covers two scenarios that show up across the Tharga ecosystem:

1. **Cached, filter-driven type discovery** over the currently-loaded assemblies (`IAssemblyService`).
2. **Filter-based DI registration** that scans for implementations of a base type and registers them all (`IServiceCollection.AddTransient/Scoped/Singleton(filter)`).

Pick the one that fits — they can also be combined.

## Install

```
dotnet add package Tharga.Runtime
```

Targets `net8.0`, `net9.0`, `net10.0`.

## Scenario 1 — Cached type lookups

Register `IAssemblyService` once, then resolve filter results by cache key:

```csharp
using Tharga.Runtime;

builder.Services.AddAssemblyService();
```

```csharp
public class HandlerRegistry(IAssemblyService assemblyService)
{
    public TypeInfo[] FindHandlers() =>
        assemblyService.GetTypes(
            cacheKey: "handlers",
            filter:   x => x.IsOfType(typeof(IHandler)) && !x.IsAbstract);
}
```

The first call with a `filter` populates the cache. Subsequent calls with the same `cacheKey` return the cached array — the filter is ignored. See [Assembly service](assembly-service.md).

## Scenario 2 — Filter-based DI

Skip the cache and register every concrete implementation of a base type in one call:

```csharp
using Tharga.Runtime;

builder.Services.AddSingleton(x => x.IsOfType<IHandler>());
```

`Add*` finds the matching implementations, resolves their direct interfaces, and adds them to the container. See [Service registration](service-registration.md).

## Next

- [Assembly service](assembly-service.md) — caching, `LoadTypes`, scoping by assembly.
- [Service registration](service-registration.md) — lifetimes, `findInterface`, `GetServiceTypePairs`.
- [Type extensions](type-extensions.md) — `IsOfType`, version-tolerant `GetType(string)`, interface helpers.
