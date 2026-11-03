using ClickToCart.Orders.Domain;

namespace ClickToCart.Orders.Application;

public sealed record CreateOrderRequest(string ProductId, int Quantity, string? LinkCode, string? VisitorId);

public sealed record OrderDto(
    string Id, string ProductId, string ProductName, int Quantity, decimal Total, DateTime CreatedAt, string? LinkCode);

public static class OrderMapper
{
    public static OrderDto ToDto(Order o) =>
        new(o.Id, o.ProductId, o.ProductName, o.Quantity, o.Total, o.CreatedAt, o.Ad?.LinkCode);
}
