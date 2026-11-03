using ClickToCart.Worker.Application;
using ClickToCart.Worker.Infrastructure;

namespace ClickToCart.Worker;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkerModule(this IServiceCollection services)
    {
        services.AddStores();
        services.AddHandlers();
        services.AddBackgroundJobs();
        return services;
    }

    private static void AddStores(this IServiceCollection services)
    {
        services.AddSingleton<IClickStore, InMemoryClickStore>();
        services.AddSingleton<IAttributedOrderStore, InMemoryAttributedOrderStore>();
        services.AddSingleton<IAdSpendStore, InMemoryAdSpendStore>();
        services.AddSingleton<IEventQueue, ChannelEventQueue>();
        services.AddSingleton<IAdSpendProvider, FakeAdSpendProvider>();
        services.AddSingleton<IMetaConversionsClient, LoggingMetaConversionsClient>();
        services.AddSingleton<DataSeeder>();
    }

    private static void AddHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IEventHandler, ClickIngestHandler>();
        services.AddSingleton<IEventHandler, AttributionHandler>();
        services.AddSingleton<EventDispatcher>();
        services.AddSingleton<AdSyncService>();
        services.AddSingleton<MetricsQuery>();
    }

    private static void AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddHostedService<EventProcessorService>();
        services.AddHostedService<AdSyncJob>();
    }
}
