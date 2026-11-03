using ClickToCart.Worker.Application;
using ClickToCart.Worker.Domain;

namespace ClickToCart.Worker.Infrastructure;

/// <summary>Fills the in-memory stores so the dashboard has data on first start.</summary>
public sealed class DataSeeder(IClickStore clicks, IAttributedOrderStore orders, TimeProvider clock)
{
    public void Seed()
    {
        var now = clock.GetUtcNow().UtcDateTime;
        SeedClicks("eid7x", "c-eid", 40, now);
        SeedClicks("eid9k", "c-eid", 30, now);
        SeedClicks("case1", "c-school", 60, now);
        SeedOrders(now);
    }

    private void SeedClicks(string link, string campaign, int count, DateTime now)
    {
        for (var i = 0; i < count; i++)
            clicks.Add(new ClickRecord(link, campaign, $"seed-{link}-{i}", null, now.AddHours(-i * 3)));
    }

    private void SeedOrders(DateTime now)
    {
        AddOrder("o-seed001", "eid7x", 3000m, now.AddDays(-1));
        AddOrder("o-seed002", "eid7x", 3000m, now.AddDays(-2));
        AddOrder("o-seed003", "case1", 200m, now.AddDays(-1));
        AddOrder("o-seed004", "case1", 200m, now.AddDays(-2));
        AddOrder("o-seed005", "case1", 200m, now.AddDays(-3));
    }

    private void AddOrder(string id, string link, decimal revenue, DateTime at) =>
        orders.Add(new AttributedOrder(id, link, revenue, at));
}
