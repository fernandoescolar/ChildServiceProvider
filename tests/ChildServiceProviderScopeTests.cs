namespace ChildServiceProviderTests;

using static TestArtifacts;

public class ChildServiceProviderScopeTests
{
    [Fact]
    public void Create_scope_returns_child_service_scope()
    {
        var provider = CreateChildProvider();

        using var scope = provider.CreateScope();

        Assert.IsType<ChildServiceScope>(scope);
        Assert.IsType<CompositeServiceProvider>(scope.ServiceProvider);
    }

    [Fact]
    public void Scoped_services_are_different_between_scopes_and_same_within_scope()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddScoped<IParentScoped, ParentScoped>(),
            configureChild: child => child.AddScoped<IChildScoped, ChildScoped>());

        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();

        var sp1 = scope1.ServiceProvider;
        var sp2 = scope2.ServiceProvider;

        var parent1a = sp1.GetRequiredService<IParentScoped>();
        var parent1b = sp1.GetRequiredService<IParentScoped>();
        var parent2 = sp2.GetRequiredService<IParentScoped>();

        var child1a = sp1.GetRequiredService<IChildScoped>();
        var child1b = sp1.GetRequiredService<IChildScoped>();
        var child2 = sp2.GetRequiredService<IChildScoped>();

        Assert.Equal(parent1a.Id, parent1b.Id);
        Assert.Equal(child1a.Id, child1b.Id);
        Assert.NotEqual(parent1a.Id, parent2.Id);
        Assert.NotEqual(child1a.Id, child2.Id);
    }

    [Fact]
    public void Child_scope_resolves_dependencies_from_parent_scope()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddScoped<IDependencyClass, DependencyClass>(),
            configureChild: child => child.AddScoped<IDependantClass, DependantClass>());

        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;

        var dependant = sp.GetRequiredService<IDependantClass>();
        var dependency = sp.GetRequiredService<IDependencyClass>();

        Assert.Equal(dependency.Id, dependant.DependencyId);
    }

    [Fact]
    public void Parent_scope_resolves_dependencies_from_child_scope()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddScoped<IDependantClass, DependantClass>(),
            configureChild: child => child.AddScoped<IDependencyClass, DependencyClass>());

        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;

        var dependant = sp.GetRequiredService<IDependantClass>();
        var dependency = sp.GetRequiredService<IDependencyClass>();

        Assert.Equal(dependency.Id, dependant.DependencyId);
    }
}
