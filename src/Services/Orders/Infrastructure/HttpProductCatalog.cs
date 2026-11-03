using System.Net;
using System.Net.Http.Json;
using ClickToCart.Orders.Application;
using ClickToCart.Shared.Contracts;
using ClickToCart.Shared.Errors;

namespace ClickToCart.Orders.Infrastructure;

public sealed class HttpProductCatalog(HttpClient http) : IProductCatalog
{
    public async Task<ProductDto> GetAsync(string productId, CancellationToken ct)
    {
        using var response = await http.GetAsync($"/api/products/{productId}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) throw new NotFoundException("Product", productId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProductDto>(ct))!;
    }
}
