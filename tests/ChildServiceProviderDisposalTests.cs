namespace ChildServiceProviderTests;

using static TestArtifacts;

public class ChildServiceProviderDisposalTests
{
    [Fact]
    public void Disposing_child_scope_disposes_scoped_services()
    {
        DisposableService.Instances.Clear();

        var provider = CreateChildProvider(
            configureParent: parent => parent.AddScoped<DisposableService>());

        DisposableService? instance;

        using (var scope = provider.CreateScope())
        {
            var sp = scope.ServiceProvider;
            instance = sp.GetRequiredService<DisposableService>();
            Assert.False(instance.Disposed);
        }

        Assert.NotNull(instance);
        Assert.True(instance!.Disposed);
    }

    [Fact]
    public async Task Disposing_child_scope_async_calls_async_dispose_if_available()
    {
        var flag = new AsyncDisposableFlag();

        var provider = CreateChildProvider(
            configureParent: parent => parent.AddScoped(_ => flag));

        await using (var scope = (ChildServiceScope)provider.CreateScope())
        {
            var sp = scope.ServiceProvider;
            var resolved = sp.GetRequiredService<AsyncDisposableFlag>();
            Assert.False(resolved.DisposedAsync);
        }

        Assert.True(flag.DisposedAsync);
    }

    [Fact]
    public void Disposing_child_provider_disposes_child_singletons()
    {
        DisposableService.Instances.Clear();

        var provider = CreateChildProvider(
            configureChild: child => child.AddSingleton<DisposableService>());

        var instance = provider.GetRequiredService<DisposableService>();

        provider.Dispose();

        Assert.True(instance.Disposed);
    }
}
