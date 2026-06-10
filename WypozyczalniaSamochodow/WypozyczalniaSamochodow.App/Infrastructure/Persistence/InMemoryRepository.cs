namespace WypozyczalniaSamochodow.App.Infrastructure.Persistence;

internal abstract class InMemoryRepository<T>
{
    private readonly List<T> _items = new();

    protected IReadOnlyList<T> Items => _items;

    public IReadOnlyList<T> All => _items.ToList();

    public void Add(T item) => _items.Add(item);

    public void Remove(T item) => _items.Remove(item);
}
