using ClickToCart.Shared.Events;

namespace ClickToCart.Shared.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken ct = default)
        where TEvent : IIntegrationEvent;
}
