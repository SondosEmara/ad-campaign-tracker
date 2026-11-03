using System.Threading.Channels;
using ClickToCart.Shared.Events;
using ClickToCart.Worker.Application;

namespace ClickToCart.Worker.Infrastructure;

/// <summary>In-process queue. Replace with RabbitMQ/Kafka by implementing IEventQueue.</summary>
public sealed class ChannelEventQueue : IEventQueue
{
    private readonly Channel<IIntegrationEvent> _channel = Channel.CreateUnbounded<IIntegrationEvent>();

    public ValueTask EnqueueAsync(IIntegrationEvent integrationEvent, CancellationToken ct) =>
        _channel.Writer.WriteAsync(integrationEvent, ct);

    public IAsyncEnumerable<IIntegrationEvent> ReadAllAsync(CancellationToken ct) =>
        _channel.Reader.ReadAllAsync(ct);
}
