# Service registration

`Tharga.Runtime` adds filter-based extension methods on `IServiceCollection`. They scan the relevant assemblies, find every type matching the filter, and register each one against the interfaces it directly implements.

## The four entry points

```csharp
using Tharga.Runtime;

services.AddTransient(x => x.IsOfType<IHandler>());
services.AddScoped   (x => x.IsOfType<IHandler>());
services.AddSingleton(x => x.IsOfType<IHandler>());
services.Add(RegistrationType.Singleton, x => x.IsOfType<IHandler>());
```

Each one accepts a `Func<TypeInfo, bool>` filter and adds one DI registration per `(ServiceType, ImplementationType)` pair it finds. Interfaces and abstract types are filtered out as implementations automatically — only concrete classes get registered.

## Generic overloads

If the base type is always the same, pass it as the type parameter instead of repeating it in the filter:

```csharp
services.AddTransient<IHandler>(x => !x.IsAbstract);
```

The generic overload composes `IsOfType<IHandler>()` into the filter for you.

## `findInterface`

Default is `true` — implementations are registered against their **directly implemented** interfaces:

```csharp
public interface IHandler { }
public class OrderHandler : IHandler { }

services.AddTransient<IHandler>(x => true);
// Resolves: IHandler → OrderHandler
```

Pass `findInterface: false` to register concrete types as themselves:

```csharp
services.AddTransient<IHandler>(x => true, findInterface: false);
// Resolves: OrderHandler → OrderHandler
```

"Directly implemented" means the interface is declared on the type itself, not inherited via another interface — see [`IsInterfaceDirectlyImplemented`](type-extensions.md#isinterfacedirectlyimplemented).

## Assembly scope

The optional `assemblies` and `baseAssembly` parameters work the same way as on `IAssemblyService` — default is the entry assembly plus same-namespace-root assemblies in the app domain. Pass an explicit list when you need a different scope:

```csharp
services.AddTransient<IHandler>(
    filter: x => !x.IsAbstract,
    assemblies: [typeof(OrderHandler).Assembly, typeof(InvoiceHandler).Assembly]);
```

## `GetServiceTypePairs` — inspect before registering

If you want to see what would be registered (for logging, diagnostics, or custom registration logic), `GetServiceTypePairs` returns the pairs without touching the container:

```csharp
var pairs = ServiceCollectionExtensions
    .GetServiceTypePairs<IHandler>(x => !x.IsAbstract);

foreach (var (serviceType, implementationType) in pairs)
{
    Console.WriteLine($"{serviceType.Name} → {implementationType.Name}");
}
```

## `RegistrationType`

```csharp
public enum RegistrationType
{
    Transient,
    Scoped,
    Singleton
}
```

Passed to the non-generic `Add(...)` overload when the lifetime is computed at runtime.
