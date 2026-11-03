namespace ClickToCart.Shared.Events;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}

/// <summary>Raised by Marketing when a visitor clicks a short link.</summary>
public sealed record ClickRecordedEvent(string LinkCode, string CampaignId, string VisitorId, string? Fbclid)
    : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}

/// <summary>Raised by Orders after an order is saved. Ad fields are empty for normal orders.</summary>
public sealed record OrderPlacedEvent(string OrderId, string ProductId, decimal Total, string? LinkCode, string? VisitorId)
    : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}

public static class EventRoutes
{
    public const string ClickRecorded = "/events/click-recorded";
    public const string OrderPlaced = "/events/order-placed";

    public static string For(IIntegrationEvent integrationEvent) => integrationEvent switch
    {
        ClickRecordedEvent => ClickRecorded,
        OrderPlacedEvent => OrderPlaced,
        _ => throw new ArgumentException($"No route for {integrationEvent.GetType().Name}")
    };
}
