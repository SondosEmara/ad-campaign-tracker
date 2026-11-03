using ClickToCart.Orders.Application;
using ClickToCart.Orders.Domain;

namespace ClickToCart.Orders.Infrastructure;

public sealed class InMemoryOrderRepository(TimeProvider clock) : IOrderRepository
{
    private readonly List<Order> _orders = [.. OrderSeed.Build(clock.GetUtcNow().UtcDateTime)];
    private readonly object _gate = new();

    public Task AddAsync(Order order, CancellationToken ct)
    {
        lock (_gate) _orders.Add(order);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct)
    {
        lock (_gate) return Task.FromResult<IReadOnlyList<Order>>(_orders.ToList());
    }
}
