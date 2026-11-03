namespace ClickToCart.Products.Domain;

public sealed record Product(string Id, string Name, decimal Price, string PageUrl);
