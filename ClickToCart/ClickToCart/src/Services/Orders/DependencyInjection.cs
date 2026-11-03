using ClickToCart.Orders.Application;
using ClickToCart.Orders.Infrastructure;
using ClickToCart.Shared.Messaging;

namespace ClickToCart.Orders;

public static class DependencyInjection
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddHttpClient<IProductCatalog, HttpProductCatalog>(client =>
            client.BaseAddress = new Uri(config["Services:Products"] ?? "http://localhost:5101"));
        services.AddEventPublisher(config);
        return services;
    }
}
