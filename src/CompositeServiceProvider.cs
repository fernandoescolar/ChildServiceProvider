namespace Microsoft.Extensions.DependencyInjection;

public class CompositeServiceProvider : IServiceProvider, IKeyedServiceProvider
{
    private readonly IServiceProvider _primary;
    private readonly IServiceProvider _secondary;

    internal CompositeServiceProvider(IServiceProvider primary, IServiceProvider secondary)
    {
        _primary = primary;
        _secondary = secondary;
    }

    public virtual object? GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        return _primary.GetService(serviceType) ?? _secondary.GetService(serviceType);
    }

    public virtual object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        if (_primary is IKeyedServiceProvider primaryKeyed)
        {
            var primaryResult = primaryKeyed.GetKeyedService(serviceType, serviceKey);
            if (primaryResult is not null)
            {
                return primaryResult;
            }
        }
        else if (serviceKey is null)
        {
            var fallback = _primary.GetService(serviceType);
            if (fallback is not null)
            {
                return fallback;
            }
        }

        if (_secondary is IKeyedServiceProvider secondaryKeyed)
        {
            return secondaryKeyed.GetKeyedService(serviceType, serviceKey);
        }

        return serviceKey is null ? _secondary.GetService(serviceType) : null;
    }

    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
    {
        var resolved = GetKeyedService(serviceType, serviceKey);
        if (resolved is null)
        {
            throw new InvalidOperationException($"Unable to resolve keyed service of type '{serviceType}' with key '{serviceKey ?? "<null>"}'.");
        }

        return resolved;
    }
}
