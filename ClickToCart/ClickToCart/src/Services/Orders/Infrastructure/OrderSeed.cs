using ClickToCart.Orders.Domain;

namespace ClickToCart.Orders.Infrastructure;

/// <summary>Demo orders. Link codes match the Marketing and Worker seeds.</summary>
internal static class OrderSeed
{
    public static IReadOnlyList<Order> Build(DateTime now) =>
    [
        Make("o-seed001", "p-1", "Air Fryer", 3000m, 1, 1, now, "eid7x"),
        Make("o-seed002", "p-1", "Air Fryer", 3000m, 1, 2, now, "eid7x"),
        Make("o-seed003", "p-2", "Phone Case", 200m, 1, 1, now, "case1"),
        Make("o-seed004", "p-2", "Phone Case", 200m, 1, 2, now, "case1"),
        Make("o-seed005", "p-2", "Phone Case", 200m, 1, 3, now, "case1"),
        Make("o-seed006", "p-3", "Wireless Headphones", 1500m, 1, 3, now),
        Make("o-seed007", "p-5", "Coffee Maker", 1800m, 1, 4, now),
        Make("o-seed008", "p-2", "Phone Case", 200m, 2, 4, now)
    ];

    private static Order Make(
        string id, string productId, string name, decimal price, int qty, int daysAgo, DateTime now, string? link = null) =>
        new(id, productId, name, qty, price * qty, now.AddDays(-daysAgo),
            link is null ? null : new AdTouch(link, $"seed-{id}"));
}
