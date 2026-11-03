using ClickToCart.Orders.Application;

namespace ClickToCart.Orders.Api;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");
        group.MapGet("/", (IOrderService service, CancellationToken ct) => service.ListAsync(ct));
        group.MapPost("/", CreateAsync);
        return app;
    }

    private static async Task<IResult> CreateAsync(CreateOrderRequest request, IOrderService service, CancellationToken ct)
    {
        var order = await service.CreateAsync(request, ct);
        return Results.Created($"/api/orders/{order.Id}", order);
    }
}
