namespace ChildServiceProviderTests;

public class ChildServiceProviderBehaviorTests
{
    [Fact]
    public void ServiceProvider_Is_ServiceProvider()
    {
        // Arrange
        var childProvider = TestArtifacts.CreateChildProvider();
        // Act
        var serviceProvider = childProvider.GetService<IServiceProvider>();
        // Assert
        Assert.Same(childProvider, serviceProvider);
    }

    [Fact]
    public void ServiceScopeFactory_Is_ServiceScopeFactory()
    {
        // Arrange
        var childProvider = TestArtifacts.CreateChildProvider();
        // Act
        var scopeFactory = childProvider.GetService<IServiceScopeFactory>();
        // Assert
        Assert.Same(childProvider, scopeFactory);
    }

    [Fact]
    public void KeyedServiceProvider_Is_KeyedServiceProvider()
    {
        // Arrange
        var childProvider = TestArtifacts.CreateChildProvider();
        // Act
        var keyedServiceProvider = childProvider.GetService<IKeyedServiceProvider>();
        // Assert
        Assert.Same(childProvider, keyedServiceProvider);
    }
}
