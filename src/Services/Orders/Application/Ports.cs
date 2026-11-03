using ClickToCart.Orders.Domain;
using ClickToCart.Shared.Contracts;

namespace ClickToCart.Orders.Application;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct);
}

/// <summary>Port to the Products microservice.</summary>
public interface IProductCatalog
{
    Task<ProductDto> GetAsync(string productId, CancellationToken ct);
}

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct);
    Task<IReadOnlyList<OrderDto>> ListAsync(CancellationToken ct);
}
