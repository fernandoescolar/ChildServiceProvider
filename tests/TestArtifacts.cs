namespace ChildServiceProviderTests;

internal static class TestArtifacts
{
    internal static ChildServiceProvider CreateChildProvider(
        Action<IServiceCollection>? configureParent = null,
        Action<IServiceCollection>? configureChild = null)
    {
        var parentServices = new ServiceCollection();
        configureParent?.Invoke(parentServices);

        var childServices = new ChildServiceCollection(parentServices);
        configureChild?.Invoke(childServices);

        var parentProvider = parentServices.BuildServiceProvider();

        return new ChildServiceProvider(parentProvider, childServices);
    }

    internal interface IFoo { }
    internal sealed class ParentFoo : IFoo { }
    internal sealed class ChildFoo : IFoo { }

    internal interface IGeneric<T>
    {
        Type Type { get; }
    }

    internal sealed class ParentGeneric<T> : IGeneric<T>
    {
        public Type Type => typeof(T);
    }

    internal sealed class ChildGeneric<T> : IGeneric<T>
    {
        public Type Type => typeof(T);
    }

    internal interface IParentScoped
    {
        Guid Id { get; }
    }

    internal interface IChildScoped
    {
        Guid Id { get; }
    }

    internal sealed class ParentScoped : IParentScoped
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    internal sealed class ChildScoped : IChildScoped
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    internal sealed class DisposableService : IDisposable
    {
        public static List<DisposableService> Instances { get; } = new();
        public bool Disposed { get; private set; }

        public DisposableService()
        {
            Instances.Add(this);
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }

    internal interface IDependencyClass
    {
        Guid Id { get; }
    }

    internal interface IDependantClass
    {
        Guid Id { get; }
        Guid DependencyId { get; }
    }

    internal sealed class DependencyClass : IDependencyClass
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    internal sealed class DependantClass : IDependantClass
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Guid DependencyId { get; }

        public DependantClass(IDependencyClass dependency)
        {
            DependencyId = dependency.Id;
        }
    }

    internal sealed class AsyncDisposableFlag : IAsyncDisposable
    {
        public bool DisposedAsync { get; private set; }

        public ValueTask DisposeAsync()
        {
            DisposedAsync = true;
            return ValueTask.CompletedTask;
        }
    }
}
