using System.Net.Http.Json;
using ClickToCart.Shared.Events;
using Microsoft.Extensions.Logging;

namespace ClickToCart.Shared.Messaging;

/// <summary>
/// Publishes events to the Worker over HTTP. A broker (RabbitMQ/Kafka) can replace this
/// class without touching any caller because callers depend on <see cref="IEventPublisher"/>.
/// </summary>
public sealed class HttpEventPublisher(HttpClient http, ILogger<HttpEventPublisher> logger) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken ct = default)
        where TEvent : IIntegrationEvent
    {
        try
        {
            await SendAsync(integrationEvent, ct);
            logger.LogInformation("Published {EventType} {EventId}", typeof(TEvent).Name, integrationEvent.EventId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not publish {EventType} {EventId}", typeof(TEvent).Name, integrationEvent.EventId);
        }
    }

    private async Task SendAsync<TEvent>(TEvent integrationEvent, CancellationToken ct)
        where TEvent : IIntegrationEvent
    {
        using var response = await http.PostAsJsonAsync(EventRoutes.For(integrationEvent), integrationEvent, ct);
        response.EnsureSuccessStatusCode();
    }
}
