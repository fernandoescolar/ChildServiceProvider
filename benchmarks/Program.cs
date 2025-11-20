using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");
var summary = BenchmarkRunner.Run<ChildServiceProviderBenchmarks>();

[SimpleJob(launchCount: 1, warmupCount: 5, iterationCount: 10)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class ChildServiceProviderBenchmarks
{
    private IServiceProvider _defaultProvider;
    private IServiceProvider _childProvider;
    private IServiceProvider _childProviderAllParent;
    private IServiceProvider _childProviderAllChild;
    private IServiceProvider _childProviderServiceParent;
    private IServiceProvider _childProviderDependencyParent;

    [GlobalSetup]
    public void Setup()
    {
        // Default ServiceProvider
        var defaultServices = new ServiceCollection();
        defaultServices.AddSingleton<ISingletonService, SingletonService>();
        defaultServices.AddSingleton<ISingletonDependency, SingletonDependency>();
        defaultServices.AddSingleton<ISingletonServiceWithDependency, SingletonServiceWithDependency>();
        defaultServices.AddScoped<IScopedService, ScopedService>();
        defaultServices.AddScoped<IScopedDependency, ScopedDependency>();
        defaultServices.AddScoped<IScopedServiceWithDependency, ScopedServiceWithDependency>();
        defaultServices.AddTransient<ITransientService, TransientService>();
        defaultServices.AddTransient<ITransientDependency, TransientDependency>();
        defaultServices.AddTransient<ITransientServiceWithDependency, TransientServiceWithDependency>();
        _defaultProvider = defaultServices.BuildServiceProvider();

        // ChildServiceProvider - standard setup
        var parentServices = new ServiceCollection();
        parentServices.AddSingleton<ISingletonService, SingletonService>();
        parentServices.AddSingleton<ISingletonDependency, SingletonDependency>();
        parentServices.AddSingleton<ISingletonServiceWithDependency, SingletonServiceWithDependency>();
        parentServices.AddScoped<IScopedService, ScopedService>();
        parentServices.AddScoped<IScopedDependency, ScopedDependency>();
        parentServices.AddScoped<IScopedServiceWithDependency, ScopedServiceWithDependency>();
        parentServices.AddTransient<ITransientService, TransientService>();
        parentServices.AddTransient<ITransientDependency, TransientDependency>();
        parentServices.AddTransient<ITransientServiceWithDependency, TransientServiceWithDependency>();
        var parent = parentServices.BuildServiceProvider();
        _childProvider = parentServices.CreateChildServiceCollection().BuildChildServiceProvider(parent);

        // All in child
        var childServices = new ServiceCollection();
        childServices.AddSingleton<ISingletonServiceWithDependency, SingletonServiceWithDependency>();
        childServices.AddSingleton<ISingletonDependency, SingletonDependency>();
        childServices.AddScoped<IScopedServiceWithDependency, ScopedServiceWithDependency>();
        childServices.AddScoped<IScopedDependency, ScopedDependency>();
        childServices.AddTransient<ITransientServiceWithDependency, TransientServiceWithDependency>();
        childServices.AddTransient<ITransientDependency, TransientDependency>();
        _childProviderAllChild = new ServiceCollection().BuildServiceProvider().BuildChildServiceProvider(childServices.CreateChildServiceCollection());

        // All in parent
        _childProviderAllParent = childServices.CreateChildServiceCollection().BuildChildServiceProvider(parent);

        // Service in parent, dependency in child
        var parentServices2 = new ServiceCollection();
        parentServices2.AddSingleton<ISingletonServiceWithDependency, SingletonServiceWithDependency>();
        parentServices2.AddScoped<IScopedServiceWithDependency, ScopedServiceWithDependency>();
        parentServices2.AddTransient<ITransientServiceWithDependency, TransientServiceWithDependency>();
        var childServices2 = parentServices2.CreateChildServiceCollection();
        childServices2.AddSingleton<ISingletonDependency, SingletonDependency>();
        childServices2.AddScoped<IScopedDependency, ScopedDependency>();
        childServices2.AddTransient<ITransientDependency, TransientDependency>();
        _childProviderServiceParent = childServices2.BuildChildServiceProvider(parentServices2.BuildServiceProvider());

        // Dependency in parent, service in child
        var parentServices3 = new ServiceCollection();
        parentServices3.AddSingleton<ISingletonDependency, SingletonDependency>();
        parentServices3.AddScoped<IScopedDependency, ScopedDependency>();
        parentServices3.AddTransient<ITransientDependency, TransientDependency>();
        var childServices3 = parentServices3.CreateChildServiceCollection();
        childServices3.AddSingleton<ISingletonServiceWithDependency, SingletonServiceWithDependency>();
        childServices3.AddScoped<IScopedServiceWithDependency, ScopedServiceWithDependency>();
        childServices3.AddTransient<ITransientServiceWithDependency, TransientServiceWithDependency>();
        _childProviderDependencyParent = parentServices3.BuildServiceProvider().BuildChildServiceProvider(childServices3);
    }

    // Singleton benchmarks
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Singleton", "Simple")]
    public void Default_Singleton_Simple() => _defaultProvider.GetService<ISingletonService>();

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Singleton", "WithDependency")]
    public void Default_Singleton_WithDependency() => _defaultProvider.GetService<ISingletonServiceWithDependency>();

    [Benchmark]
    [BenchmarkCategory("Singleton", "Simple")]
    public void Child_Singleton_Simple() => _childProvider.GetService<ISingletonService>();

    [Benchmark]
    [BenchmarkCategory("Singleton", "WithDependency")]
    public void Child_Singleton_WithDependency_AllParent() => _childProviderAllParent.GetService<ISingletonServiceWithDependency>();

    [Benchmark]
    [BenchmarkCategory("Singleton", "WithDependency")]
    public void Child_Singleton_WithDependency_AllChild() => _childProviderAllChild.GetService<ISingletonServiceWithDependency>();

    [Benchmark]
    [BenchmarkCategory("Singleton", "WithDependency")]
    public void Child_Singleton_WithDependency_ServiceParent() => _childProviderServiceParent.GetService<ISingletonServiceWithDependency>();

    [Benchmark]
    [BenchmarkCategory("Singleton", "WithDependency")]
    public void Child_Singleton_WithDependency_DependencyParent() => _childProviderDependencyParent.GetService<ISingletonServiceWithDependency>();

    // Scoped benchmarks
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Scoped", "Simple")]
    public void Default_Scoped_Simple()
    {
        using var scope = _defaultProvider.CreateScope();
        scope.ServiceProvider.GetService<IScopedService>();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Scoped", "WithDependency")]
    public void Default_Scoped_WithDependency()
    {
        using var scope = _defaultProvider.CreateScope();
        scope.ServiceProvider.GetService<IScopedServiceWithDependency>();
    }

    [Benchmark]
    [BenchmarkCategory("Scoped", "Simple")]
    public void Child_Scoped_Simple()
    {
        using var scope = _childProvider.CreateScope();
        scope.ServiceProvider.GetService<IScopedService>();
    }

    [Benchmark]
    [BenchmarkCategory("Scoped", "WithDependency")]
    public void Child_Scoped_WithDependency_AllParent()
    {
        using var scope = _childProviderAllParent.CreateScope();
        scope.ServiceProvider.GetService<IScopedServiceWithDependency>();
    }

    [Benchmark]
    [BenchmarkCategory("Scoped", "WithDependency")]
    public void Child_Scoped_WithDependency_AllChild()
    {
        using var scope = _childProviderAllChild.CreateScope();
        scope.ServiceProvider.GetService<IScopedServiceWithDependency>();
    }

    [Benchmark]
    [BenchmarkCategory("Scoped", "WithDependency")]
    public void Child_Scoped_WithDependency_ServiceParent()
    {
        using var scope = _childProviderServiceParent.CreateScope();
        scope.ServiceProvider.GetService<IScopedServiceWithDependency>();
    }

    [Benchmark]
    [BenchmarkCategory("Scoped", "WithDependency")]
    public void Child_Scoped_WithDependency_DependencyParent()
    {
        using var scope = _childProviderDependencyParent.CreateScope();
        scope.ServiceProvider.GetService<IScopedServiceWithDependency>();
    }

    // Transient benchmarks
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Transient", "Simple")]
    public void Default_Transient_Simple() => _defaultProvider.GetService<ITransientService>();

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Transient", "WithDependency")]
    public void Default_Transient_WithDependency() => _defaultProvider.GetService<ITransientServiceWithDependency>();

    [Benchmark]
    [BenchmarkCategory("Transient", "Simple")]
    public void Child_Transient_Simple() => _childProvider.GetService<ITransientService>();

    [Benchmark]
    [BenchmarkCategory("Transient", "WithDependency")]
    public void Child_Transient_WithDependency_AllParent() => _childProviderAllParent.GetService<ITransientServiceWithDependency>();

    [Benchmark]
    [BenchmarkCategory("Transient", "WithDependency")]
    public void Child_Transient_WithDependency_AllChild() => _childProviderAllChild.GetService<ITransientServiceWithDependency>();

    [Benchmark]
    [BenchmarkCategory("Transient", "WithDependency")]
    public void Child_Transient_WithDependency_ServiceParent() => _childProviderServiceParent.GetService<ITransientServiceWithDependency>();

    [Benchmark]
    [BenchmarkCategory("Transient", "WithDependency")]
    public void Child_Transient_WithDependency_DependencyParent() => _childProviderDependencyParent.GetService<ITransientServiceWithDependency>();

    // Service interfaces and implementations
    public interface ISingletonService { }
    public class SingletonService : ISingletonService { }

    public interface ISingletonDependency { }
    public class SingletonDependency : ISingletonDependency { }

    public interface ISingletonServiceWithDependency { }
    public class SingletonServiceWithDependency : ISingletonServiceWithDependency
    {
        public SingletonServiceWithDependency(ISingletonDependency dependency) { }
    }

    public interface IScopedService { }
    public class ScopedService : IScopedService { }

    public interface IScopedDependency { }
    public class ScopedDependency : IScopedDependency { }

    public interface IScopedServiceWithDependency { }
    public class ScopedServiceWithDependency : IScopedServiceWithDependency
    {
        public ScopedServiceWithDependency(IScopedDependency dependency) { }
    }

    public interface ITransientService { }
    public class TransientService : ITransientService { }

    public interface ITransientDependency { }
    public class TransientDependency : ITransientDependency { }

    public interface ITransientServiceWithDependency { }
    public class TransientServiceWithDependency : ITransientServiceWithDependency
    {
        public TransientServiceWithDependency(ITransientDependency dependency) { }
    }
}
