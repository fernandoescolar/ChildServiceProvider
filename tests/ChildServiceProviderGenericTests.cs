namespace ChildServiceProviderTests;

using static TestArtifacts;

public class ChildServiceProviderGenericTests
{
    [Fact]
    public void Uses_parent_closed_generic_over_child_open_generic()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddSingleton<IGeneric<int>, ParentGeneric<int>>(),
            configureChild: child => child.AddSingleton(typeof(IGeneric<>), typeof(ChildGeneric<>)));

        var resolved = provider.GetRequiredService<IGeneric<int>>();

        Assert.IsType<ParentGeneric<int>>(resolved);
    }

        [Fact]
    public void Uses_child_closed_generic_over_parent_open_generic()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddSingleton(typeof(IGeneric<>), typeof(ParentGeneric<>)),
            configureChild: child => child.AddSingleton<IGeneric<int>, ChildGeneric<int>>());

        var resolved = provider.GetRequiredService<IGeneric<int>>();

        Assert.IsType<ChildGeneric<int>>(resolved);
    }

    [Fact]
    public void Uses_parent_closed_generic_when_child_does_not_override()
    {
        var provider = CreateChildProvider(
            configureParent: parent => parent.AddSingleton<IGeneric<int>, ParentGeneric<int>>());

        var resolved = provider.GetRequiredService<IGeneric<int>>();

        Assert.IsType<ParentGeneric<int>>(resolved);
    }

    [Fact]
    public void Child_open_generic_serves_multiple_types()
    {
        var provider = CreateChildProvider(
            configureChild: child => child.AddSingleton(typeof(IGeneric<>), typeof(ChildGeneric<>)));

        var intGeneric = provider.GetRequiredService<IGeneric<int>>();
        var stringGeneric = provider.GetRequiredService<IGeneric<string>>();

        Assert.Equal(typeof(int), intGeneric.Type);
        Assert.Equal(typeof(string), stringGeneric.Type);
        Assert.IsType<ChildGeneric<int>>(intGeneric);
        Assert.IsType<ChildGeneric<string>>(stringGeneric);
    }
}
