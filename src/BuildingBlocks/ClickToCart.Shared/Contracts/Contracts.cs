namespace ClickToCart.Shared.Contracts;

/// <summary>Product data shared between services (Products API response).</summary>
public sealed record ProductDto(string Id, string Name, decimal Price, string PageUrl);

/// <summary>Per-link numbers produced by the Worker and read by Marketing reporting.</summary>
public sealed record LinkMetricsDto(string LinkCode, int Clicks, int Orders, decimal Revenue, decimal Spend)
{
    public static LinkMetricsDto Empty(string linkCode) => new(linkCode, 0, 0, 0m, 0m);
}
