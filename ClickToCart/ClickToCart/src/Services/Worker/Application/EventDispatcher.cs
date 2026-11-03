using ClickToCart.Shared.Events;
using Microsoft.Extensions.Logging;

namespace ClickToCart.Worker.Application;

public sealed class EventDispatcher(IEnumerable<IEventHandler> handlers, ILogger<EventDispatcher> logger)
{
    public async Task DispatchAsync(IIntegrationEvent integrationEvent, CancellationToken ct)
    {
        var matching = handlers.Where(h => h.CanHandle(integrationEvent)).ToList();
        if (matching.Count == 0) logger.LogWarning("No handler for {EventType}", integrationEvent.GetType().Name);

        foreach (var handler in matching) await RunAsync(handler, integrationEvent, ct);
    }

    private async Task RunAsync(IEventHandler handler, IIntegrationEvent integrationEvent, CancellationToken ct)
    {
        try
        {
            await handler.HandleAsync(integrationEvent, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Handler} failed for event {EventId}", handler.GetType().Name, integrationEvent.EventId);
        }
    }
}
