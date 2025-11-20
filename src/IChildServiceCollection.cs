namespace Microsoft.Extensions.DependencyInjection;

public interface IChildServiceCollection : IServiceCollection
{
    IServiceCollection ParentServices { get; }
    IServiceCollection ChildServices { get; }
}
