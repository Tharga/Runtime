# Type extensions

A small set of `Type` and `TypeInfo` helpers used by the rest of `Tharga.Runtime` and exposed for direct use.

## `IsOfType`

```csharp
bool IsOfType<T>(this Type item);
bool IsOfType(this Type item, Type type);
```

Returns `true` if `item` is, inherits from, or implements `type`. Walks the base-type chain and implemented interfaces. Comparison strips generic argument differences (`List<int>` matches `List<>`).

```csharp
typeof(OrderHandler).IsOfType<IHandler>();   // true
typeof(List<int>).IsOfType(typeof(IList<>)); // true
```

Used as the primary building block for [DI filters](service-registration.md) and [assembly scans](assembly-service.md).

## `GetDirectlyImplementedInterfaces`

```csharp
Type[] GetDirectlyImplementedInterfaces(this Type type);
```

Returns only the interfaces declared on `type` itself — interfaces inherited via another interface are excluded.

```csharp
public interface IBase { }
public interface IDerived : IBase { }
public class Impl : IDerived { }

typeof(Impl).GetInterfaces();                       // [IDerived, IBase]
typeof(Impl).GetDirectlyImplementedInterfaces();    // [IDerived]
```

## `IsInterfaceDirectlyImplemented`

```csharp
bool IsInterfaceDirectlyImplemented(this Type type, Type typeInterface);
```

Returns `true` if `typeInterface` is declared directly on `type`, not inherited via another interface. Throws `ArgumentException` if `typeInterface` is not an interface.

Used by the DI registration code so that `Impl : IDerived` registers only against `IDerived` — not also against the inherited `IBase`.

## `ToAssemblyQualifiedNameWithoutVersion`

```csharp
string ToAssemblyQualifiedNameWithoutVersion(this Type type);
```

Returns `"Namespace.Type, Assembly.Name"` — the assembly-qualified name with `Version=`, `Culture=`, and `PublicKeyToken=` stripped. Useful for serializing type references that need to survive assembly version bumps.

```csharp
typeof(MyType).ToAssemblyQualifiedNameWithoutVersion();
// "MyApp.MyType, MyApp"
```

## `GetType(string)` — version-tolerant

```csharp
Type GetType(string typeName);
```

Wraps `Type.GetType(string)` with a fallback: tries the provided assembly-qualified name first with `Version=1.0.0.0` substituted in, then with whatever version is in the string. Throws `TypeMissingException` if the type can't be resolved.

This is intentionally forgiving — designed for loading types from configuration or stored data where the recorded version may no longer match the loaded one.

## `HasGenericParameter`

```csharp
bool HasGenericParameter(this TypeInfo type, Type aggregatorType);
```

Walks `type`'s base-type and interface chain looking for a generic type, then checks whether the first generic argument is `aggregatorType`.

## `GetGenericTypeOf<T>`

```csharp
Type GetGenericTypeOf<T>(this Type type);
```

Returns the first generic argument anywhere in `type`'s inheritance chain that is assignable to `T`, or `null` if none is found.

```csharp
public class MyRepo : Repository<Order> { }

typeof(MyRepo).GetGenericTypeOf<IEntity>(); // typeof(Order), if Order : IEntity
```
