# Assembly service

`IAssemblyService` caches filtered type lookups across the currently-loaded assemblies. It exists for the case where you need the same `TypeInfo[]` repeatedly — `GetTypes(cacheKey, …)` returns the cached array on every call after the first.

## Registration

```csharp
using Tharga.Runtime;

builder.Services.AddAssemblyService();
```

`AddAssemblyService` registers `IAssemblyService` as a **singleton**. The cache is process-wide.

## Lookup

Two forms — one that primes the cache on first call, one that requires a prior `LoadTypes`.

```csharp
// Self-priming: filter is used the first time, ignored after.
var handlers = assemblyService.GetTypes(
    "handlers",
    x => x.IsOfType<IHandler>() && !x.IsAbstract);

// Pre-primed: filterless GetTypes requires an earlier LoadTypes
// with the same key, otherwise it throws InvalidOperationException.
assemblyService.LoadTypes("handlers", x => x.IsOfType<IHandler>() && !x.IsAbstract);

// Anywhere later:
var cached = assemblyService.GetTypes("handlers");
```

Calling `GetTypes(key)` (no filter) before the key has been populated throws — there's nothing to return.

## How the assembly set is determined

By default the scan covers the **entry assembly** plus every assembly in `AppDomain.CurrentDomain` whose `FullName` starts with the same root namespace segment as the entry assembly (e.g. `Tharga.*` if the entry assembly is `Tharga.Foo`). This is a deliberate trade-off — restricting to one namespace family avoids scanning unrelated SDK and framework assemblies, which is faster and avoids `ReflectionTypeLoadException` from unrelated dependencies.

Override the scope with `assemblies` (explicit list) or `baseAssembly` (different starting point):

```csharp
assemblyService.LoadTypes(
    "handlers",
    x => x.IsOfType<IHandler>(),
    baseAssembly: typeof(SomeMarker).Assembly);
```

## Static helpers

For one-off scans where you don't want the cache, the static methods on `AssemblyService` are usable directly:

```csharp
var allHandlers = AssemblyService.GetTypes<IHandler>(filter: x => !x.IsAbstract);
var loaded      = AssemblyService.GetLoadedAssemblies();
```

## When to use the cache vs. the static helpers

| Scenario | Use |
|---|---|
| Same filter resolved repeatedly during the request/operation lifetime | `IAssemblyService.GetTypes(key, filter)` |
| Single scan at startup (e.g. DI registration) | `AssemblyService.GetTypes(filter)` or the [`IServiceCollection` filter extensions](service-registration.md) |
| Lookup by a key shared across components | `LoadTypes` once, then `GetTypes(key)` everywhere |
