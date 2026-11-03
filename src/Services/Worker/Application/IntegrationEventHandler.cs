using ClickToCart.Shared.Events;

namespace ClickToCart.Worker.Application;

public interface IEventHandler
{
    bool CanHandle(IIntegrationEvent integrationEvent);
    Task HandleAsync(IIntegrationEvent integrationEvent, CancellationToken ct);
}

/// <summary>Typed base so each handler only deals with one event type (open for new handlers).</summary>
public abstract class IntegrationEventHandler<TEvent> : IEventHandler where TEvent : IIntegrationEvent
{
    public bool CanHandle(IIntegrationEvent integrationEvent) => integrationEvent is TEvent;

    public Task HandleAsync(IIntegrationEvent integrationEvent, CancellationToken ct) =>
        HandleEventAsync((TEvent)integrationEvent, ct);

    protected abstract Task HandleEventAsync(TEvent integrationEvent, CancellationToken ct);
}
