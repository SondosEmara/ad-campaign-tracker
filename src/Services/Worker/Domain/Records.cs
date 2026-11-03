namespace ClickToCart.Worker.Domain;

public sealed record ClickRecord(string LinkCode, string CampaignId, string VisitorId, string? Fbclid, DateTime ClickedAt);

public sealed record AttributedOrder(string OrderId, string LinkCode, decimal Revenue, DateTime AttributedAt);

public sealed record OrderSummary(int Orders, decimal Revenue);

/// <summary>An order is credited to an ad only if the buyer clicked it within the last 7 days.</summary>
public static class AttributionPolicy
{
    public static readonly TimeSpan Window = TimeSpan.FromDays(7);

    public static bool IsWithinWindow(DateTime clickedAt, DateTime orderedAt)
    {
        var elapsed = orderedAt - clickedAt;
        return elapsed >= TimeSpan.Zero && elapsed <= Window;
    }
}
