using ClickToCart.Shared.Events;
using ClickToCart.Worker.Domain;
using Microsoft.Extensions.Logging;

namespace ClickToCart.Worker.Application;

/// <summary>Attribution: decides which ad (if any) gets credit for an order.</summary>
public sealed class AttributionHandler(
    IClickStore clicks,
    IAttributedOrderStore orders,
    IMetaConversionsClient meta,
    ILogger<AttributionHandler> logger) : IntegrationEventHandler<OrderPlacedEvent>
{
    protected override async Task HandleEventAsync(OrderPlacedEvent e, CancellationToken ct)
    {
        if (!IsAdOrder(e)) return;
        if (orders.Exists(e.OrderId)) { logger.LogInformation("Order {OrderId} already attributed", e.OrderId); return; }
        if (!HasValidClick(e)) { logger.LogInformation("Order {OrderId} has no click inside the window", e.OrderId); return; }

        await AttributeAsync(e, ct);
    }

    private static bool IsAdOrder(OrderPlacedEvent e) =>
        !string.IsNullOrWhiteSpace(e.LinkCode) && !string.IsNullOrWhiteSpace(e.VisitorId);

    private bool HasValidClick(OrderPlacedEvent e)
    {
        var click = clicks.FindLatest(e.LinkCode!, e.VisitorId!);
        return click is not null && AttributionPolicy.IsWithinWindow(click.ClickedAt, e.OccurredAt);
    }

    private async Task AttributeAsync(OrderPlacedEvent e, CancellationToken ct)
    {
        var attributed = new AttributedOrder(e.OrderId, e.LinkCode!, e.Total, e.OccurredAt);
        orders.Add(attributed);
        logger.LogInformation("Order {OrderId} ({Total}) attributed to link {Link}", e.OrderId, e.Total, e.LinkCode);
        await meta.SendPurchaseAsync(attributed, ct);
    }
}
