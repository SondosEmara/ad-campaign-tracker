using ClickToCart.Products.Domain;
using ClickToCart.Shared.Contracts;

namespace ClickToCart.Products.Application;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct);
    Task<Product?> FindAsync(string id, CancellationToken ct);
}

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> ListAsync(CancellationToken ct);
    Task<ProductDto> GetAsync(string id, CancellationToken ct);
}
