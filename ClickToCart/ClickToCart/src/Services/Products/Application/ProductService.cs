using ClickToCart.Products.Domain;
using ClickToCart.Shared.Contracts;
using ClickToCart.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace ClickToCart.Products.Application;

public sealed class ProductService(IProductRepository repository, ILogger<ProductService> logger) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> ListAsync(CancellationToken ct)
    {
        var products = await repository.GetAllAsync(ct);
        logger.LogInformation("Listed {Count} products", products.Count);
        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto> GetAsync(string id, CancellationToken ct)
    {
        var product = await repository.FindAsync(id, ct) ?? throw new NotFoundException("Product", id);
        return ToDto(product);
    }

    private static ProductDto ToDto(Product p) => new(p.Id, p.Name, p.Price, p.PageUrl);
}
