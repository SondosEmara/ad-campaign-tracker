using ClickToCart.Shared.Events;
using ClickToCart.Worker.Domain;
using Microsoft.Extensions.Logging;

namespace ClickToCart.Worker.Application;

/// <summary>Click Ingest: stores each click so attribution and reporting can use it.</summary>
public sealed class ClickIngestHandler(IClickStore clicks, ILogger<ClickIngestHandler> logger)
    : IntegrationEventHandler<ClickRecordedEvent>
{
    protected override Task HandleEventAsync(ClickRecordedEvent e, CancellationToken ct)
    {
        clicks.Add(new ClickRecord(e.LinkCode, e.CampaignId, e.VisitorId, e.Fbclid, e.OccurredAt));
        logger.LogInformation("Click stored: link {Link}, visitor {Visitor}", e.LinkCode, e.VisitorId);
        return Task.CompletedTask;
    }
}
