using Microsoft.Extensions.Logging;

namespace ClickToCart.Worker.Application;

/// <summary>Ad Sync: pulls ad spend per link from Meta and stores it for reporting.</summary>
public sealed class AdSyncService(
    IClickStore clicks, IAdSpendStore spendStore, IAdSpendProvider provider, ILogger<AdSyncService> logger)
{
    public async Task SyncAsync(CancellationToken ct)
    {
        var clicksByLink = clicks.CountByLink();
        foreach (var (linkCode, count) in clicksByLink)
            spendStore.Set(linkCode, await provider.GetSpendAsync(linkCode, count, ct));

        logger.LogInformation("Ad spend synced for {Count} links", clicksByLink.Count);
    }
}
