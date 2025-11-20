namespace Microsoft.Extensions.DependencyInjection;

internal sealed class ChildServiceScope : IServiceScope, IAsyncDisposable
{
    private readonly IServiceScope _parentScope;
    private readonly IServiceScope _innerScope;
    private readonly IServiceProvider _serviceProvider;
    private bool _disposed;

    public ChildServiceScope(IServiceScope parentScope, IServiceScope innerScope)
    {
        _parentScope = parentScope ?? throw new ArgumentNullException(nameof(parentScope));
        _innerScope = innerScope ?? throw new ArgumentNullException(nameof(innerScope));
        _serviceProvider = new CompositeServiceProvider(_innerScope.ServiceProvider, _parentScope.ServiceProvider);
    }

    public IServiceProvider ServiceProvider => _serviceProvider;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _innerScope.Dispose();
        _parentScope.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_innerScope is IAsyncDisposable asyncInner)
        {
            await asyncInner.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            _innerScope.Dispose();
        }

        if (_parentScope is IAsyncDisposable asyncParent)
        {
            await asyncParent.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            _parentScope.Dispose();
        }
    }
}

