using ClickToCart.Products.Domain;

namespace ClickToCart.Products.Infrastructure;

internal static class ProductSeed
{
    private const string Shop = "https://shopeg.com/p";

    public static IReadOnlyList<Product> All { get; } =
    [
        new("p-1", "Air Fryer", 3000m, $"{Shop}/air-fryer"),
        new("p-2", "Phone Case", 200m, $"{Shop}/phone-case"),
        new("p-3", "Wireless Headphones", 1500m, $"{Shop}/headphones"),
        new("p-4", "Smart Watch", 2500m, $"{Shop}/smart-watch"),
        new("p-5", "Coffee Maker", 1800m, $"{Shop}/coffee-maker")
    ];
}
