namespace Microsoft.Extensions.DependencyInjection;

public sealed partial class ChildServiceProvider : CompositeServiceProvider, IServiceProvider, IKeyedServiceProvider, IServiceScopeFactory, IDisposable, IAsyncDisposable
{
    private readonly IServiceProvider _parentProvider;
    private readonly IServiceProvider _innerProvider;
    private readonly IChildServiceCollection _childServices;
    private readonly bool _ownsInnerProvider;
    private bool _disposed;

    internal ChildServiceProvider(IServiceProvider parentProvider, IChildServiceCollection childServices)
        : this(
            parentProvider,
            childServices,
            childServices.BuildServiceProvider(),
            true)
    {
    }

    internal ChildServiceProvider(
        IServiceProvider parentProvider,
        IChildServiceCollection childServices,
        IServiceProvider innerProvider,
        bool ownsInnerProvider) : base(innerProvider, parentProvider)
    {
        _parentProvider = parentProvider ?? throw new ArgumentNullException(nameof(parentProvider));
        _childServices = childServices ?? throw new ArgumentNullException(nameof(childServices));
        _innerProvider = innerProvider ?? throw new ArgumentNullException(nameof(innerProvider));
        _ownsInnerProvider = ownsInnerProvider;
    }

    public IServiceScope CreateScope()
    {
        var parentScopeFactory = _parentProvider.GetRequiredService<IServiceScopeFactory>();
        var parentScope = parentScopeFactory.CreateScope();

        var innerScopeFactory = _innerProvider.GetRequiredService<IServiceScopeFactory>();
        var innerScope = innerScopeFactory.CreateScope();

        return new ChildServiceScope(parentScope, innerScope);
    }

    public override object? GetService(Type serviceType)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(serviceType);

        if (serviceType == typeof(IServiceScopeFactory) || serviceType == typeof(IServiceProvider) || serviceType == typeof(IKeyedServiceProvider))
        {
            return this;
        }

        return base.GetService(serviceType);
    }

    public override object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(serviceType);

        return base.GetKeyedService(serviceType, serviceKey);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_ownsInnerProvider && _innerProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return ValueTask.CompletedTask;
        }

        _disposed = true;

        if (!_ownsInnerProvider)
        {
            return ValueTask.CompletedTask;
        }

        if (_innerProvider is IAsyncDisposable asyncDisposable)
        {
            return asyncDisposable.DisposeAsync();
        }

        if (_innerProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        return ValueTask.CompletedTask;
    }
}
