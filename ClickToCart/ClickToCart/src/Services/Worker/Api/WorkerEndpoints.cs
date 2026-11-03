using ClickToCart.Shared.Events;
using ClickToCart.Worker.Application;

namespace ClickToCart.Worker.Api;

public static class WorkerEndpoints
{
    public static IEndpointRouteBuilder MapWorkerEndpoints(this IEndpointRouteBuilder app)
    {
        MapEvent<ClickRecordedEvent>(app, EventRoutes.ClickRecorded);
        MapEvent<OrderPlacedEvent>(app, EventRoutes.OrderPlaced);
        app.MapGet("/api/metrics/links", (MetricsQuery query) => query.GetLinkMetrics()).WithTags("Metrics");
        return app;
    }

    private static void MapEvent<TEvent>(IEndpointRouteBuilder app, string route) where TEvent : IIntegrationEvent =>
        app.MapPost(route, async (TEvent integrationEvent, IEventQueue queue, CancellationToken ct) =>
        {
            await queue.EnqueueAsync(integrationEvent, ct);
            return Results.Accepted();
        }).WithTags("Events");
}
