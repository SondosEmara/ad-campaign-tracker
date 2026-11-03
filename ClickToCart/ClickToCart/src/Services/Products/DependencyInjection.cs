using ClickToCart.Products.Application;
using ClickToCart.Products.Infrastructure;

namespace ClickToCart.Products;

public static class DependencyInjection
{
    public static IServiceCollection AddProductsModule(this IServiceCollection services)
    {
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}
