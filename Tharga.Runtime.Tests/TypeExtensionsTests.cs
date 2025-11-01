using FluentAssertions;
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
}