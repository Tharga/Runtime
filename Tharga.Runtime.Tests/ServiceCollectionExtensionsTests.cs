using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;
using Xunit;

namespace Tharga.Runtime.Tests;

public class ServiceCollectionExtensionsTests
{
    private readonly List<ServiceDescriptor> _items;
    private readonly Mock<IServiceCollection> _services;

    public ServiceCollectionExtensionsTests()
    {
        _items = new List<ServiceDescriptor>();
        _services = new Mock<IServiceCollection>(MockBehavior.Strict);
        _services.Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
            .Callback((ServiceDescriptor x) => _items.Add(x));
    }

    [Fact]
    public void SimpleService()
    {
        //Arrange

        //Act
        _services.Object.Add(RegistrationType.Transient, x => x.IsOfType<SimpleService>(), baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)));

        //Assert
        _items.Should().HaveCount(1);
        _items.Single().ServiceType.Should().Be<SimpleService>();
        _items.Single().ImplementationType.Should().Be<SimpleService>();
    }

    [Fact]
    public void ServiceWithInterface()
    {
        //Arrange

        //Act
        _services.Object.Add(RegistrationType.Transient, x => x.IsOfType<ServiceWithInterface>(), baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)), findInterface: true);

        //Assert
        _items.Should().HaveCount(1);
        _items.Single().ServiceType.Should().Be<IServiceInterface>();
        _items.Single().ImplementationType.Should().Be<ServiceWithInterface>();
    }

    [Fact]
    public void ServiceWithInterfaceIgnoreInterface()
    {
        //Arrange

        //Act
        _services.Object.Add(RegistrationType.Transient, x => x.IsOfType<ServiceWithInterface>(), baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)), findInterface: false);

        //Assert
        _items.Should().HaveCount(1);
        _items.Single().ServiceType.Should().Be<ServiceWithInterface>();
        _items.Single().ImplementationType.Should().Be<ServiceWithInterface>();
    }

    [Fact]
    public void ServiceByInterface()
    {
        //Arrange

        //Act
        _services.Object.Add<IServiceInterface>(RegistrationType.Transient, _ => true, baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)));

        //Assert
        _items.Should().HaveCount(1);
        _items.Single().ServiceType.Should().Be<IServiceInterface>();
        _items.Single().ImplementationType.Should().Be<ServiceWithInterface>();
    }

    [Fact]
    public void ServiceWithBase()
    {
        //Arrange

        //Act
        _services.Object.Add<ServiceWithBase>(RegistrationType.Transient, _ => true, baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)));

        //Assert
        _items.Should().HaveCount(1);
        _items.Single().ServiceType.Should().Be<ServiceWithBase>();
        _items.Single().ImplementationType.Should().Be<ServiceWithBase>();
    }

    [Fact]
    public void ServiceByBase()
    {
        //Arrange

        //Act
        _services.Object.Add<ServiceBase>(RegistrationType.Transient, _ => true, baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)));

        //Assert
        _items.Should().HaveCount(3);
        _items[0].ServiceType.Should().Be<ServiceWithBase>();
        _items[0].ImplementationType.Should().Be<ServiceWithBase>();
        _items[1].ServiceType.Should().Be<IServiceInterface2>();
        _items[1].ImplementationType.Should().Be<ServiceWithBaseAndInterface>();
        _items[2].ServiceType.Should().Be<ServiceWithBaseButNoInterface>();
        _items[2].ImplementationType.Should().Be<ServiceWithBaseButNoInterface>();
    }

    [Fact]
    public void ServiceWithBaseByInterface()
    {
        //Arrange

        //Act
        _services.Object.Add<IServiceInterface2>(RegistrationType.Transient, _ => true, baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)));

        //Assert
        _items.Should().HaveCount(1);
        _items.Single().ServiceType.Should().Be<IServiceInterface2>();
        _items.Single().ImplementationType.Should().Be<ServiceWithBaseAndInterface>();
    }

    [Fact]
    public void ServiceWithBaseByInterface2()
    {
        //Arrange

        //Act
        _services.Object.Add<IServiceInterface2>(RegistrationType.Transient, _ => true, baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)), findInterface: false);

        //Assert
        _items.Should().HaveCount(1);
        _items.Single().ServiceType.Should().Be<ServiceWithBaseAndInterface>();
        _items.Single().ImplementationType.Should().Be<ServiceWithBaseAndInterface>();
    }

    [Fact]
    public void ServiceByCommonInterface()
    {
        //Arrange

        //Act
        _services.Object.Add<ICommonServiceInterface>(RegistrationType.Transient, _ => true, baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)));

        //Assert
        _items.Should().HaveCount(2);
        _items.First().ServiceType.Should().Be<ICommonServiceInterface>();
        _items.First().ImplementationType.Should().Be<Service1WithCommonInterface>();
        _items.Last().ServiceType.Should().Be<ICommonServiceInterface>();
        _items.Last().ImplementationType.Should().Be<Service2WithCommonInterface>();
    }

    [Fact]
    public void ServiceWithTwoInterfaces()
    {
        //Arrange

        //Act
        _services.Object.Add<ServiceWithTwoInterfaces>(RegistrationType.Transient, _ => true, baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)));

        //Assert
        _items.Should().HaveCount(2);
        _items.First().ServiceType.Should().Be<IServiceInterfaceA>();
        _items.First().ImplementationType.Should().Be<ServiceWithTwoInterfaces>();
        _items.Last().ServiceType.Should().Be<IServiceInterfaceB>();
        _items.Last().ImplementationType.Should().Be<ServiceWithTwoInterfaces>();
    }

    [Fact]
    public void RepositoryRegistrationSimulator()
    {
        //Arrange

        //Act
        _services.Object.Add<IRepository>(RegistrationType.Transient, _ => true, baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)));

        //Assert
        _items.Should().HaveCount(2);
        _items.First().ServiceType.Should().Be<IRepositoryA>();
        _items.First().ImplementationType.Should().Be<RepositoryA>();
        _items.Last().ServiceType.Should().Be<IRepositoryB>();
        _items.Last().ImplementationType.Should().Be<RepositoryB>();
    }

    [Fact]
    public void GetServiceTypePairs()
    {
        //Arrange
        Func<TypeInfo, bool> filter = x => x.IsOfType<ServiceBase>();

        //Act
        var types = ServiceCollectionExtensions.GetServiceTypePairs(filter, baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests))).ToArray();

        //Assert
        types.Should().HaveCount(3);
    }

    [Fact]
    public void GetServiceTypePairsByBaseType()
    {
        //Arrange

        //Act
        var types = ServiceCollectionExtensions.GetServiceTypePairs<ServiceBase>(_ => true, baseAssembly: Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests))).ToArray();

        //Assert
        types.Should().HaveCount(3);
    }
}

public class SimpleService
{
}

public interface IServiceInterface
{
}

public class ServiceWithInterface : IServiceInterface
{
}

public abstract class ServiceBase
{
}

public class ServiceWithBase : ServiceBase
{
}

public interface IServiceInterface2
{
}

public class ServiceWithBaseAndInterface : ServiceBase, IServiceInterface2
{
}

public class ServiceWithBaseButNoInterface : ServiceBase
{
}

public interface ICommonServiceInterface
{
}

public class Service1WithCommonInterface : ICommonServiceInterface
{
}

public class Service2WithCommonInterface : ICommonServiceInterface
{
}

public interface IServiceInterfaceA
{
}

public interface IServiceInterfaceB
{
}

public class ServiceWithTwoInterfaces : IServiceInterfaceA, IServiceInterfaceB
{
}

public interface IRepository
{
}

public interface IRepositoryA : IRepository
{
}

public class RepositoryA : IRepositoryA
{
}

public interface IRepositoryB : IRepository
{
}

public class RepositoryB : IRepositoryB
{
}