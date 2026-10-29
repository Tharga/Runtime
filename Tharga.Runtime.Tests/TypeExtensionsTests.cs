using AwesomeAssertions;
using Xunit;

namespace Tharga.Runtime.Tests;

public class TypeExtensionsTests
{
    [Fact]
    public void SimpleService()
    {
        //Arrange
        var type = typeof(SimpleService);

        //Act

        //Assert
        type.IsOfType<SimpleService>().Should().BeTrue();
        type.IsOfType<object>().Should().BeTrue();
    }

    [Fact]
    public void ServiceWithInterface()
    {
        //Arrange
        var type = typeof(ServiceWithInterface);

        //Act

        //Assert
        type.IsOfType<IServiceInterface>().Should().BeTrue();
        type.IsOfType<ServiceWithInterface>().Should().BeTrue();
    }

    [Fact]
    public void ServiceInterface()
    {
        //Arrange
        var type = typeof(IServiceInterface);

        //Act

        //Assert
        type.IsOfType<IServiceInterface>().Should().BeTrue();
        type.IsOfType<ServiceWithInterface>().Should().BeFalse();
    }

    [Fact]
    public void ClosedGenericDoesNotMatchAnotherClosedFormOfTheSameGeneric()
    {
        typeof(List<int>).IsOfType(typeof(List<string>)).Should().BeFalse();
        typeof(IsOfTypeHandlerB).IsOfType(typeof(IIsOfTypeHandler<IsOfTypeA>)).Should().BeFalse();
    }

    [Fact]
    public void ArrayDoesNotMatchItsElementType()
    {
        typeof(int[]).IsOfType(typeof(int)).Should().BeFalse();
        typeof(int[]).IsOfType(typeof(IEnumerable<int>)).Should().BeTrue();
    }

    [Fact]
    public void OpenGenericDefinitionMatchesItsClosedForms()
    {
        typeof(List<int>).IsOfType(typeof(List<>)).Should().BeTrue();
        typeof(List<int>).IsOfType(typeof(IList<>)).Should().BeTrue();
        typeof(IsOfTypeHandlerA).IsOfType(typeof(IIsOfTypeHandler<>)).Should().BeTrue();
        typeof(IsOfTypeDerived).IsOfType(typeof(IsOfTypeBase<>)).Should().BeTrue();
    }

    [Fact]
    public void ClosedGenericMatchesTheSameClosedForm()
    {
        typeof(IsOfTypeHandlerA).IsOfType(typeof(IIsOfTypeHandler<IsOfTypeA>)).Should().BeTrue();
        typeof(IsOfTypeDerived).IsOfType(typeof(IsOfTypeBase<IsOfTypeA>)).Should().BeTrue();
    }

    [Fact]
    public void FilterOnClosedGenericFindsOnlyThatClosedForm()
    {
        var pairs = ServiceCollectionExtensions.GetServiceTypePairs(x => x.IsOfType<IIsOfTypeHandler<IsOfTypeA>>(), baseAssembly: typeof(TypeExtensionsTests).Assembly).ToArray();

        pairs.Should().Contain((typeof(IIsOfTypeHandler<IsOfTypeA>), typeof(IsOfTypeHandlerA)));
        pairs.Should().NotContain(x => x.ImplementationType == typeof(IsOfTypeHandlerB));
    }

    [Fact]
    public void GetTypeResolvesNameWhereVersionIsTheLastPart()
    {
        TypeExtensions.GetType("System.String, System.Private.CoreLib, Version=9.0.0.0").Should().Be(typeof(string));
    }

    [Fact]
    public void GetTypeResolvesGenericNameWhereVersionIsTheLastPart()
    {
        var typeName = $"System.Collections.Generic.List`1[[{typeof(string).AssemblyQualifiedName}]], System.Private.CoreLib, Version=9.0.0.0";

        TypeExtensions.GetType(typeName).Should().Be(typeof(List<string>));
    }

    [Fact]
    public void GetTypeResolvesAssemblyQualifiedName()
    {
        TypeExtensions.GetType(typeof(string).AssemblyQualifiedName).Should().Be(typeof(string));
        TypeExtensions.GetType(typeof(List<string>).AssemblyQualifiedName).Should().Be(typeof(List<string>));
    }
}

public interface IIsOfTypeHandler<T>;

public class IsOfTypeA;

public class IsOfTypeB;

public class IsOfTypeHandlerA : IIsOfTypeHandler<IsOfTypeA>;

public class IsOfTypeHandlerB : IIsOfTypeHandler<IsOfTypeB>;

public class IsOfTypeBase<T>;

public class IsOfTypeDerived : IsOfTypeBase<IsOfTypeA>;