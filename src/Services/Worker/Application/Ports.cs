using ClickToCart.Shared.Events;
using ClickToCart.Worker.Domain;

namespace ClickToCart.Worker.Application;

public interface IClickStore
{
    void Add(ClickRecord click);
    ClickRecord? FindLatest(string linkCode, string visitorId);
    IReadOnlyDictionary<string, int> CountByLink();
}

public interface IAttributedOrderStore
{
    bool Exists(string orderId);
    void Add(AttributedOrder order);
    IReadOnlyDictionary<string, OrderSummary> SummaryByLink();
}

public interface IAdSpendStore
{
    void Set(string linkCode, decimal spend);
    IReadOnlyDictionary<string, decimal> GetAll();
}

public interface IEventQueue
{
    ValueTask EnqueueAsync(IIntegrationEvent integrationEvent, CancellationToken ct);
    IAsyncEnumerable<IIntegrationEvent> ReadAllAsync(CancellationToken ct);
}

/// <summary>Port to Meta (Facebook/Instagram) Marketing API for ad spend.</summary>
public interface IAdSpendProvider
{
    Task<decimal> GetSpendAsync(string linkCode, int clicks, CancellationToken ct);
}

/// <summary>Port to the Meta Conversions API (server-side Purchase events).</summary>
public interface IMetaConversionsClient
{
    Task SendPurchaseAsync(AttributedOrder order, CancellationToken ct);
}
