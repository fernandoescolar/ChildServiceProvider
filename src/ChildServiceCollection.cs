namespace Microsoft.Extensions.DependencyInjection;

public sealed class ChildServiceCollection(IServiceCollection parent) : IChildServiceCollection
{
    public IServiceCollection ParentServices { get; } = parent ?? throw new ArgumentNullException(nameof(parent));

    public IServiceCollection ChildServices { get; } = new ServiceCollection();

    public ServiceDescriptor this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return index < ParentServices.Count
                ? ParentServices[index]
                : ChildServices[index - ParentServices.Count];
        }
        set
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (index < ParentServices.Count)
            {
                throw new NotSupportedException("Cannot modify parent service descriptors.");
            }

            ChildServices[index - ParentServices.Count] = value;
        }
    }

    public int Count => ParentServices.Count + ChildServices.Count;

    public bool IsReadOnly => false;

    public void Add(ServiceDescriptor item) => ChildServices.Add(item);

    public void Clear() => ChildServices.Clear();

    public bool Contains(ServiceDescriptor item) => ChildServices.Contains(item) || ParentServices.Contains(item);

    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (arrayIndex < 0 || arrayIndex > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("The target array is not large enough to contain the collection.", nameof(array));
        }

        var offset = arrayIndex;
        foreach (var descriptor in ParentServices)
        {
            array[offset++] = descriptor;
        }

        foreach (var descriptor in ChildServices)
        {
            array[offset++] = descriptor;
        }
    }

    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        foreach (var descriptor in ParentServices)
        {
            yield return descriptor;
        }

        foreach (var descriptor in ChildServices)
        {
            yield return descriptor;
        }
    }

    public int IndexOf(ServiceDescriptor item)
    {
        var parentIndex = ParentServices.IndexOf(item);
        if (parentIndex >= 0)
        {
            return parentIndex;
        }

        var innerIndex = ChildServices.IndexOf(item);
        return innerIndex >= 0 ? ParentServices.Count + innerIndex : -1;
    }

    public void Insert(int index, ServiceDescriptor item)
    {
        if (index < 0 || index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (index < ParentServices.Count)
        {
            throw new NotSupportedException("Cannot insert before parent service descriptors.");
        }

        ChildServices.Insert(index - ParentServices.Count, item);
    }

    public bool Remove(ServiceDescriptor item)
    {
        if (ChildServices.Remove(item))
        {
            return true;
        }

        if (ParentServices.Contains(item))
        {
            throw new NotSupportedException("Cannot remove parent service descriptors.");
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (index < ParentServices.Count)
        {
            throw new NotSupportedException("Cannot remove parent service descriptors.");
        }

        ChildServices.RemoveAt(index - ParentServices.Count);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
