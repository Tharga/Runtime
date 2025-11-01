# Tharga Runtime
[![GitHub repo Issues](https://img.shields.io/github/issues/Tharga/Runtime?style=flat&logo=github&logoColor=red&label=Issues)](https://github.com/Tharga/Runtime/issues?q=is%3Aopen)

## .NET
[![NuGet](https://img.shields.io/nuget/v/Tharga.Runtime)](https://www.nuget.org/packages/Tharga.Runtime)
![Nuget](https://img.shields.io/nuget/dt/Tharga.Runtime)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Assembly and TypeService Service

Register the service
```
public void ConfigureServices(IServiceCollection services)
{
	services.AddAssemblyService();
}
```

When `GetTypes` is called the first time, data is stored in cache. All other calls will use that cache.
```
assemblyService.GetTypes("CacheKey", x => x.IsOfType(typeof([SomeType]), false) && !x.IsAbstract);
```

Preload cache by calling `LoadTypes`.
```
assemblyService.LoadTypes("CacheKey", x => x.IsOfType(typeof([SomeType]), false) && !x.IsAbstract);
```