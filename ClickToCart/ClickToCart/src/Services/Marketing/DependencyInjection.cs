using ClickToCart.Marketing.Application;
using ClickToCart.Marketing.Infrastructure;
using ClickToCart.Shared.Messaging;

namespace ClickToCart.Marketing;

public static class DependencyInjection
{
    public static IServiceCollection AddMarketingModule(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ShortLinkOptions>(config.GetSection("ShortLinks"));
        services.AddRepositories();
        services.AddApplicationServices();
        services.AddServiceClients(config);
        services.AddEventPublisher(config);
        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ICampaignRepository, InMemoryCampaignRepository>();
        services.AddSingleton<ILinkRepository, InMemoryLinkRepository>();
        services.AddSingleton<ICodeGenerator, Base62CodeGenerator>();
    }

    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<ILinkService, LinkService>();
        services.AddScoped<IRedirectService, RedirectService>();
        services.AddScoped<IReportingService, ReportingService>();
    }

    private static void AddServiceClients(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<IProductCatalog, HttpProductCatalog>(c =>
            c.BaseAddress = new Uri(config["Services:Products"] ?? "http://localhost:5101"));
        services.AddHttpClient<IMetricsClient, HttpMetricsClient>(c =>
            c.BaseAddress = new Uri(config["Services:Worker"] ?? "http://localhost:5104"));
    }
}
