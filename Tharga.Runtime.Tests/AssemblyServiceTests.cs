using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Tharga.Runtime.Tests;

public class AssemblyServiceTests
{
    [Fact]
    public void GetAssemblies()
    {
        //Act
        var assemblies = AssemblyService.GetAssemblies();

        //Assert
        var names = assemblies.Select(x => x.GetName().Name).ToArray();
        if (names.Length == 1) (names.First() == "testhost" || names.First() == "ReSharperTestRunner").Should().BeTrue($"first assembly is {names.First()}.");
        if (names.Length == 5) names.Should().Contain("nCrunch.Common.DotNetCore", $"first assembly is {names.First()}.");
    }

    [Fact]
    public void GetAssembliesByGenericType()
    {
        //Act
        var assemblies = AssemblyService.GetAssemblies<TypeMissingException>();

        //Assert
        var names = assemblies.Select(x => x.GetName().Name).ToArray();
        names.Should().HaveCount(2);
        names.Should().Contain("Tharga.Runtime");
    }


    [Fact]
    public void GetAssembliesByType()
    {
        //Act
        var assemblies = AssemblyService.GetAssemblies(Assembly.GetAssembly(typeof(TypeMissingException)));

        //Assert
        var names = assemblies.Select(x => x.GetName().Name).ToArray();
        names.Should().HaveCount(2);
        names.Should().Contain("Tharga.Runtime");
    }

    [Fact]
    public void GetTypesByBaseType()
    {
        //Act
        var types = AssemblyService.GetTypes<TypeMissingException>(baseAssembly: Assembly.GetAssembly(typeof(TypeMissingException)));

        //Assert
        types.Should().HaveCount(1);
    }

    [Fact]
    public void GetTypes()
    {
        //Act
        var types = AssemblyService.GetTypes(baseAssembly: Assembly.GetAssembly(typeof(TypeMissingException)));

        //Assert
        types.Should().HaveCountGreaterThan(40);
    }

    [Fact]
    public void LoadCache()
    {
        //Arrange
        var sut = new AssemblyService();

        //Act
        sut.LoadTypes("A", _ => true, baseAssembly: Assembly.GetAssembly(typeof(TypeMissingException)));

        //Assert
        var types = sut.GetTypes("A");
        types.Should().HaveCountGreaterThan(40);
    }

    [Fact]
    public void GetTypesWithCache()
    {
        //Arrange
        var sut = new AssemblyService();

        //Act
        var types = sut.GetTypes("A", _ => true, baseAssembly: Assembly.GetAssembly(typeof(TypeMissingException)));

        //Assert
        var l = types.Length;
        l.Should().BeGreaterThan(40);
        sut.GetTypes("A").Should().HaveCount(l);
    }

    [Fact]
    public void GetLoadedAssemblies()
    {
        //Act
        var response = AssemblyService.GetLoadedAssemblies(filter: x => x.FullName?.StartsWith("Tharga.") ?? false).ToArray();

        //Assert
        response.Should().HaveCount(2);
    }
}