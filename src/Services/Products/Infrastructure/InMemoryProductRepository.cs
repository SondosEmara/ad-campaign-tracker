using ClickToCart.Products.Application;
using ClickToCart.Products.Domain;

namespace ClickToCart.Products.Infrastructure;

/// <summary>Seeded in-memory store. Swap for a real database by implementing IProductRepository.</summary>
public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly IReadOnlyList<Product> _products = ProductSeed.All;

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct) => Task.FromResult(_products);

    public Task<Product?> FindAsync(string id, CancellationToken ct) =>
        Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
}
