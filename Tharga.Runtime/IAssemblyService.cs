using System.Reflection;

namespace Tharga.Runtime;

public interface IAssemblyService
{
    /// <summary>
    ///     Loads types into the cache. Then the types can be fetched using the GetServiceTypePairs (without a filter).
    ///     See 'GetAssemblies' in 'AssemblyService' for default behaviour.
    /// </summary>
    /// <param name="cacheKey">Unique key for the specific cache.</param>
    /// <param name="filter">Filter the types that should be loaded.</param>
    /// <param name="assemblies">Provide what assemblies should be used to find types.</param>
    /// <param name="baseAssembly">Assembly to be used as entry point to look for types.</param>
    void LoadTypes(string cacheKey, Func<TypeInfo, bool> filter, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null);

    /// <summary>
    ///     Gets types from cache. If a filter is provided the cache can be populated on the first call, otherwise LoadTypes
    ///     has to be called first to prepare the cache.
    /// </summary>
    /// <param name="cacheKey">Unique key for the specific cache.</param>
    /// <param name="filter">Filter the types that should be loaded.</param>
    /// <param name="baseAssembly">Assembly to be used as entry point to look for types.</param>
    TypeInfo[] GetTypes(string cacheKey, Func<TypeInfo, bool> filter = null, Assembly baseAssembly = null);
}