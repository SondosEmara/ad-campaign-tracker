using ClickToCart.Products.Application;

namespace ClickToCart.Products.Api;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");
        group.MapGet("/", (IProductService service, CancellationToken ct) => service.ListAsync(ct));
        group.MapGet("/{id}", (string id, IProductService service, CancellationToken ct) => service.GetAsync(id, ct));
        return app;
    }
}
