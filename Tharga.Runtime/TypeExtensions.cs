using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Runtime;

public static class TypeExtensions
{
    /// <summary>
    ///     Registers AssemblyService in services.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAssemblyService(this IServiceCollection services)
    {
        services.AddSingleton<IAssemblyService>(_ => new AssemblyService());
        return services;
    }

    /// <summary>
    ///     Creates an AssemblyQualifiedNameWithout without the version part.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ToAssemblyQualifiedNameWithoutVersion(this Type type)
    {
        var r = $"{type.FullName}, {type.Assembly.GetName().Name}";
        return r;
    }

    /// <summary>
    ///     Get the type using a AssemblyQualifiedName string. Trying to fetch with provided version or with default version
    ///     '1.0.0.0'.
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    /// <exception cref="TypeMissingException"></exception>
    public static Type GetType(string typeName)
    {
        var type = GetTypeWithVersion(typeName, "1.0.0.0") ?? GetTypeWithVersion(typeName);
        if (type == null) throw new TypeMissingException(typeName);
        return type;
    }

    private static Type GetTypeWithVersion(string typeName, string version = null)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return null;
        }

        try
        {
            var tvn = ReplaceAssemblyVersion(typeName, version);

            var t = Type.GetType(tvn);
            return t;
        }
        catch (FileLoadException)
        {
            return null;
        }
    }

    private static string ReplaceAssemblyVersion(string typeName, string version)
    {
        var tvn = typeName;
        if (!string.IsNullOrEmpty(version))
        {
            var startIndex = 0;
            while (true)
            {
                var p1 = tvn.IndexOf("Version=", startIndex, StringComparison.Ordinal);
                if (p1 == -1) break;
                var p2 = tvn.IndexOf(",", p1, StringComparison.Ordinal);
                startIndex = p2;
                tvn = tvn.Substring(0, p1 + 8) + version + tvn.Substring(p2);
            }
        }

        return tvn;
    }

    /// <summary>
    ///     Checks if a type is or is based of a certain type. Also checks interfaces and collection-types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool IsOfType<T>(this Type item)
    {
        return item.IsOfType(typeof(T));
    }

    /// <summary>
    ///     Checks if a type is or is based of a certain type. Also checks interfaces and collection-types.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsOfType(this Type item, Type type)
    {
        if (item == null) return false;
        if (item == type) return true;
        if (item.ToString().Split('[')[0] == type.ToString().Split('[')[0]) return true;

        if (type.IsAssignableFrom(item))
        {
            return true;
        }

        if (item.BaseType != null && item.BaseType != typeof(object))
        {
            return item.BaseType.IsOfType(type);
        }

        foreach (var inf in item.GetInterfaces())
        {
            var r = inf.IsOfType(type);
            if (r) return true;
        }

        return false;
    }

    /// <summary>
    ///     Checks the type, base type or interfaces have generic parameters.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="aggregatorType"></param>
    /// <returns></returns>
    public static bool HasGenericParameter(this TypeInfo type, Type aggregatorType)
    {
        var p = type;
        while (!p.IsGenericType)
        {
            var parent = p.ImplementedInterfaces.FirstOrDefault();
            if (parent == null)
            {
                parent = p.BaseType;
            }

            if (parent == null) return false;

            p = parent.GetTypeInfo();
        }

        if (!p.GenericTypeArguments.Any()) return false;
        return p.GenericTypeArguments[0] == aggregatorType;
    }

    /// <summary>
    ///     Get the first generic parameter type for provided type that is based on T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type GetGenericTypeOf<T>(this Type type)
    {
        if (type.GenericTypeArguments.Any())
        {
            foreach (var argument in type.GenericTypeArguments)
            {
                if (argument.IsOfType<T>())
                {
                    return argument;
                }
            }
        }

        return type.BaseType == null ? null : GetGenericTypeOf<T>(type.BaseType);
    }
}