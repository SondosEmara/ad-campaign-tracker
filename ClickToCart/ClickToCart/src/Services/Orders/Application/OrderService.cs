using ClickToCart.Orders.Domain;
using ClickToCart.Shared.Events;
using ClickToCart.Shared.Messaging;
using Microsoft.Extensions.Logging;

namespace ClickToCart.Orders.Application;

public sealed class OrderService(
    IOrderRepository repository,
    IProductCatalog catalog,
    IEventPublisher publisher,
    TimeProvider clock,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct)
    {
        var order = await BuildOrderAsync(request, ct);
        await repository.AddAsync(order, ct);
        logger.LogInformation("Order {OrderId} created: {Product} x{Qty} = {Total} (ad link: {Link})",
            order.Id, order.ProductName, order.Quantity, order.Total, order.Ad?.LinkCode ?? "none");
        await PublishPlacedAsync(order, ct);
        return OrderMapper.ToDto(order);
    }

    public async Task<IReadOnlyList<OrderDto>> ListAsync(CancellationToken ct)
    {
        var orders = await repository.GetAllAsync(ct);
        return orders.OrderByDescending(o => o.CreatedAt).Select(OrderMapper.ToDto).ToList();
    }

    private async Task<Order> BuildOrderAsync(CreateOrderRequest request, CancellationToken ct)
    {
        var product = await catalog.GetAsync(request.ProductId, ct);
        return Order.Create(product.Id, product.Name, product.Price, request.Quantity,
            AdTouchFrom(request), clock.GetUtcNow().UtcDateTime);
    }

    private static AdTouch? AdTouchFrom(CreateOrderRequest request) =>
        string.IsNullOrWhiteSpace(request.LinkCode) || string.IsNullOrWhiteSpace(request.VisitorId)
            ? null
            : new AdTouch(request.LinkCode, request.VisitorId);

    private Task PublishPlacedAsync(Order order, CancellationToken ct) =>
        publisher.PublishAsync(
            new OrderPlacedEvent(order.Id, order.ProductId, order.Total, order.Ad?.LinkCode, order.Ad?.VisitorId), ct);
}
