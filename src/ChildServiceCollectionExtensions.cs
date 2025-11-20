namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IChildServiceCollection CreateChildServiceCollection(this IServiceCollection parentServices)
        => new ChildServiceCollection(parentServices);

    public static IChildServiceCollection CreateChildServiceCollection(this IServiceCollection parentServices, IServiceCollection childServices)
    {
        var childCollection = new ChildServiceCollection(parentServices);
        foreach (var service in childServices)
        {
            childCollection.Add(service);
        }
        return childCollection;
    }

    public static IServiceProvider BuildChildServiceProvider(this IChildServiceCollection childServiceCollection, IServiceProvider parentServiceProvider)
        => new ChildServiceProvider(parentServiceProvider, childServiceCollection);

     public static IServiceProvider BuildChildServiceProvider(this IServiceProvider parentServiceProvider, IChildServiceCollection childServiceCollection)
        => new ChildServiceProvider(parentServiceProvider, childServiceCollection);
}
