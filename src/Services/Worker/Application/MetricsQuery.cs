using ClickToCart.Shared.Contracts;
using ClickToCart.Worker.Domain;

namespace ClickToCart.Worker.Application;

/// <summary>Read side: joins clicks, attributed orders and spend into one row per link.</summary>
public sealed class MetricsQuery(IClickStore clicks, IAttributedOrderStore orders, IAdSpendStore spend)
{
    public IReadOnlyList<LinkMetricsDto> GetLinkMetrics()
    {
        var clickCounts = clicks.CountByLink();
        var orderSummaries = orders.SummaryByLink();
        var spendByLink = spend.GetAll();

        return clickCounts.Keys.Union(orderSummaries.Keys).Union(spendByLink.Keys)
            .Select(code => Build(code, clickCounts, orderSummaries, spendByLink))
            .ToList();
    }

    private static LinkMetricsDto Build(
        string code,
        IReadOnlyDictionary<string, int> clickCounts,
        IReadOnlyDictionary<string, OrderSummary> orderSummaries,
        IReadOnlyDictionary<string, decimal> spendByLink)
    {
        var summary = orderSummaries.GetValueOrDefault(code) ?? new OrderSummary(0, 0m);
        return new LinkMetricsDto(code, clickCounts.GetValueOrDefault(code), summary.Orders,
            summary.Revenue, spendByLink.GetValueOrDefault(code));
    }
}
