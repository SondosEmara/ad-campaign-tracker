using ClickToCart.Marketing.Domain;
using ClickToCart.Shared.Errors;
using ClickToCart.Shared.Events;
using ClickToCart.Shared.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClickToCart.Marketing.Application;

public sealed class RedirectService(
    ILinkRepository links,
    ICampaignRepository campaigns,
    IEventPublisher publisher,
    IOptions<ShortLinkOptions> options,
    ILogger<RedirectService> logger) : IRedirectService
{
    public async Task<RedirectResult> ResolveAsync(ClickContext click, CancellationToken ct)
    {
        var link = await links.FindAsync(click.Code, ct) ?? throw new NotFoundException("Link", click.Code);
        var campaign = await campaigns.FindAsync(link.CampaignId, ct) ?? throw new NotFoundException("Campaign", link.CampaignId);
        var visitorId = click.VisitorId ?? NewVisitorId();

        if (!link.CanRedirect(campaign)) return Fallback(link, visitorId);

        await PublishClickAsync(link, visitorId, click.Fbclid, ct);
        return new RedirectResult(link.FinalUrl, visitorId);
    }

    private RedirectResult Fallback(ShortLink link, string visitorId)
    {
        logger.LogInformation("Link {Code} is not live; sending visitor to the fallback page", link.Code);
        return new RedirectResult(options.Value.FallbackUrl, visitorId);
    }

    private Task PublishClickAsync(ShortLink link, string visitorId, string? fbclid, CancellationToken ct)
    {
        logger.LogInformation("Click on {Code} by visitor {Visitor}", link.Code, visitorId);
        return publisher.PublishAsync(new ClickRecordedEvent(link.Code, link.CampaignId, visitorId, fbclid), ct);
    }

    private static string NewVisitorId() => $"v-{Guid.NewGuid():N}"[..12];
}
