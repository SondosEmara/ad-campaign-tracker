using System.Collections.Concurrent;
using ClickToCart.Worker.Application;
using ClickToCart.Worker.Domain;

namespace ClickToCart.Worker.Infrastructure;

public sealed class InMemoryClickStore : IClickStore
{
    private readonly List<ClickRecord> _clicks = [];
    private readonly object _gate = new();

    public void Add(ClickRecord click)
    {
        lock (_gate) _clicks.Add(click);
    }

    public ClickRecord? FindLatest(string linkCode, string visitorId)
    {
        lock (_gate)
            return _clicks.Where(c => c.LinkCode == linkCode && c.VisitorId == visitorId)
                          .MaxBy(c => c.ClickedAt);
    }

    public IReadOnlyDictionary<string, int> CountByLink()
    {
        lock (_gate) return _clicks.GroupBy(c => c.LinkCode).ToDictionary(g => g.Key, g => g.Count());
    }
}

public sealed class InMemoryAttributedOrderStore : IAttributedOrderStore
{
    private readonly ConcurrentDictionary<string, AttributedOrder> _orders = new();

    public bool Exists(string orderId) => _orders.ContainsKey(orderId);

    public void Add(AttributedOrder order) => _orders.TryAdd(order.OrderId, order);

    public IReadOnlyDictionary<string, OrderSummary> SummaryByLink() =>
        _orders.Values.GroupBy(o => o.LinkCode)
               .ToDictionary(g => g.Key, g => new OrderSummary(g.Count(), g.Sum(o => o.Revenue)));
}

public sealed class InMemoryAdSpendStore : IAdSpendStore
{
    private readonly ConcurrentDictionary<string, decimal> _spend = new();

    public void Set(string linkCode, decimal spend) => _spend[linkCode] = spend;

    public IReadOnlyDictionary<string, decimal> GetAll() => new Dictionary<string, decimal>(_spend);
}
