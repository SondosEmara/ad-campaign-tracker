using ClickToCart.Worker.Application;

namespace ClickToCart.Worker.Infrastructure;

/// <summary>Reads queued events one by one and hands them to the dispatcher.</summary>
public sealed class EventProcessorService(IEventQueue queue, EventDispatcher dispatcher, ILogger<EventProcessorService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Event processor started");
        await foreach (var integrationEvent in queue.ReadAllAsync(stoppingToken))
            await dispatcher.DispatchAsync(integrationEvent, stoppingToken);
    }
}

/// <summary>Runs the Meta ad-spend sync now and then every 30 seconds.</summary>
public sealed class AdSyncJob(AdSyncService sync, ILogger<AdSyncJob> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            await SafeSyncAsync(stoppingToken);
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task SafeSyncAsync(CancellationToken ct)
    {
        try { await sync.SyncAsync(ct); }
        catch (Exception ex) { logger.LogError(ex, "Ad spend sync failed"); }
    }
}
