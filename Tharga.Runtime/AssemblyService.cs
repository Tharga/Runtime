using System.Collections.Concurrent;
using System.Reflection;

namespace Tharga.Runtime;

public class AssemblyService : IAssemblyService
{
    private static readonly ConcurrentDictionary<string, TypeInfo[]> _cache = new();

    internal AssemblyService()
    {
    }

    public void LoadTypes(string cacheKey, Func<TypeInfo, bool> filter, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null)
    {
        var data = GetTypes(filter, assemblies, baseAssembly).ToArray();
        _cache.AddOrUpdate(cacheKey, data, (_, _) => data);
    }

    public TypeInfo[] GetTypes(string cacheKey, Func<TypeInfo, bool> filter = null, Assembly baseAssembly = null)
    {
        if (_cache.TryGetValue(cacheKey, out var data))
        {
            return data;
        }

        if (filter != null)
        {
            LoadTypes(cacheKey, filter, null, baseAssembly);
            if (_cache.TryGetValue(cacheKey, out data))
            {
                return data;
            }
        }

        throw new InvalidOperationException($"Cannot find any loaded types for '{cacheKey}'. Call {nameof(LoadTypes)} on startup or provide a {nameof(filter)}.");
    }

    /// <summary>
    /// Get all loaded assemblies.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="baseAssembly"></param>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetLoadedAssemblies(Func<Assembly, bool> filter = null, Assembly baseAssembly = null)
    {
        baseAssembly ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        var appDomainAssemblies = new[]{ baseAssembly }.Union(AppDomain.CurrentDomain.GetAssemblies())
            .Where(x => filter?.Invoke(x) ?? true);

        return appDomainAssemblies;
    }

    /// <summary>
    ///     Get all assemblies in the current app domain based on the first part of the namespace from the EntryAssembly or
    ///     ExecutingAssembly.
    ///     Example: Namespace [FirstPart].[SecondPart] will return all assemblies that starts with [FirstPart].
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetAssemblies()
    {
        return GetAssemblies(null, null);
    }

    /// <summary>
    ///     Use specific class to find the base assembly where to start looking for assemblies.
    /// </summary>
    /// <typeparam name="TAssemblyType">Type for the assembly to be used as entry point to look for types.</typeparam>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetAssemblies<TAssemblyType>()
    {
        var baseAssembly = Assembly.GetAssembly(typeof(TAssemblyType));
        return GetAssemblies(null, baseAssembly);
    }

    /// <summary>
    ///     Use specific class to find the baseAssembly assembly type.
    /// </summary>
    /// <param name="baseAssembly">
    ///     Assembly to be used as entry point to look for types. Default is EntryAssembly or
    ///     ExecutingAssembly.
    /// </param>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetAssemblies(Assembly baseAssembly)
    {
        return GetAssemblies(null, baseAssembly);
    }

    /// <summary>
    ///     Get all types based on the provided type and parameters.
    /// </summary>
    /// <typeparam name="TBaseType">Type, base type of interface.</typeparam>
    /// <param name="filter">Filter for what types to return.</param>
    /// <param name="assemblies">Assemblies where to find the types. See 'GetAssemblies' for default behaviour.</param>
    /// <param name="baseAssembly">
    ///     Assembly to be used as entry point to look for types. Default is EntryAssembly or
    ///     ExecutingAssembly.
    /// </param>
    /// <returns></returns>
    public static IEnumerable<TypeInfo> GetTypes<TBaseType>(Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null)
    {
        var asm = GetAssemblies(assemblies, baseAssembly);
        var types = asm.SelectMany(x => x.DefinedTypes)
            .Where(x => x.IsOfType<TBaseType>())
            .Where(x => filter?.Invoke(x) ?? true);
        return types;
    }

    /// <summary>
    ///     Get all types for provided parameters.
    /// </summary>
    /// <param name="filter">Filter for what types to return.</param>
    /// <param name="assemblies">Assemblies where to find the types. See 'GetAssemblies' for default behaviour.</param>
    /// <param name="baseAssembly">
    ///     Assembly to be used as entry point to look for types. Default is EntryAssembly or
    ///     ExecutingAssembly.
    /// </param>
    /// <returns></returns>
    public static IEnumerable<TypeInfo> GetTypes(Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null)
    {
        var asm = GetAssemblies(assemblies, baseAssembly);
        var types = asm.SelectMany(x => x.DefinedTypes)
            .Where(x => filter?.Invoke(x) ?? true);
        return types;
    }

    private static IEnumerable<Assembly> GetAssemblies(IEnumerable<Assembly> assemblies, Assembly baseAssembly = null)
    {
        if (assemblies != null) return assemblies.ToArray();

        baseAssembly ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var name = baseAssembly.GetName().Name?.Split('.').First();

        var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => name != null && x.FullName != null && x.FullName.Contains(name))
            .ToArray();

        return new[] { baseAssembly }.Union(appDomainAssemblies).ToArray();
    }
}