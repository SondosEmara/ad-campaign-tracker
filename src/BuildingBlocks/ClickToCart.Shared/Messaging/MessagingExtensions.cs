using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClickToCart.Shared.Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AddEventPublisher(this IServiceCollection services, IConfiguration config)
    {
        var workerUrl = config["Services:Worker"] ?? "http://localhost:5104";
        services.AddHttpClient<IEventPublisher, HttpEventPublisher>(client => client.BaseAddress = new Uri(workerUrl));
        return services;
    }
}
