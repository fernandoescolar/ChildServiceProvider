namespace ChildServiceProviderTests;

using static TestArtifacts;

public class ChildServiceProviderResolutionTests
{
    [Fact]
    public void Resolves_service_from_parent_when_child_has_no_registration()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddSingleton<IFoo, ParentFoo>());

        var result = provider.GetService(typeof(IFoo));

        Assert.IsType<ParentFoo>(result);
    }

    [Fact]
    public void Resolves_service_from_child_when_both_register_it()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddSingleton<IFoo, ParentFoo>(),
            configureChild: child => child.AddSingleton<IFoo, ChildFoo>());

        var result = provider.GetService(typeof(IFoo));

        Assert.IsType<ChildFoo>(result);
    }

    [Fact]
    public void Returns_null_when_service_is_not_registered_anywhere()
    {
        var provider = CreateChildProvider();

        var result = provider.GetService(typeof(IFoo));

        Assert.Null(result);
    }

    [Fact]
    public void Parent_resolves_dependencies_from_child()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddSingleton<IDependantClass, DependantClass>(),
            configureChild: child => child.AddSingleton<IDependencyClass, DependencyClass>());

        var dependant = (IDependantClass)provider.GetRequiredService(typeof(IDependantClass));
        var dependency = (IDependencyClass)provider.GetRequiredService(typeof(IDependencyClass));

        Assert.Equal(dependency.Id, dependant.DependencyId);
    }

    [Fact]
    public void Child_resolves_dependencies_from_parent()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddSingleton<IDependencyClass, DependencyClass>(),
            configureChild: child => child.AddSingleton<IDependantClass, DependantClass>());

        var dependant = (IDependantClass)provider.GetRequiredService(typeof(IDependantClass));
        var dependency = (IDependencyClass)provider.GetRequiredService(typeof(IDependencyClass));

        Assert.Equal(dependency.Id, dependant.DependencyId);
    }

    [Fact]
    public void Resolves_keyed_service_from_parent_when_child_has_no_registration()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddKeyedSingleton<IFoo>("foo", (sp, _) => new ParentFoo()));

        var keyedProvider = (IKeyedServiceProvider)provider;
        var result = keyedProvider.GetKeyedService(typeof(IFoo), "foo");

        Assert.IsType<ParentFoo>(result);
    }

    [Fact]
    public void Resolves_keyed_service_from_child_when_both_register_it()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddKeyedSingleton<IFoo>("shared", (sp, _) => new ParentFoo()),
            configureChild: child => child.AddKeyedSingleton<IFoo>("shared", (sp, _) => new ChildFoo()));

        var keyedProvider = (IKeyedServiceProvider)provider;
        var result = keyedProvider.GetKeyedService(typeof(IFoo), "shared");

        Assert.IsType<ChildFoo>(result);
    }

    [Fact]
    public void Child_keyed_open_generic_overrides_parent_closed_generic()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddKeyedSingleton(typeof(IGeneric<>), "key", typeof(ParentGeneric<>)),
            configureChild: child => child.AddKeyedSingleton(typeof(IGeneric<>), "key", typeof(ChildGeneric<>)));

        var keyedProvider = (IKeyedServiceProvider)provider;
        var resolved = (IGeneric<int>)keyedProvider.GetRequiredKeyedService(typeof(IGeneric<int>), "key");

        Assert.IsType<ChildGeneric<int>>(resolved);
    }
}
