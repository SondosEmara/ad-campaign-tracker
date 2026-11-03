using ClickToCart.Shared.Errors;

namespace ClickToCart.Orders.Domain;

/// <summary>Proof that the buyer arrived through an ad link (read from the ad_touch cookie).</summary>
public sealed record AdTouch(string LinkCode, string VisitorId);

public sealed record Order(
    string Id, string ProductId, string ProductName, int Quantity, decimal Total, DateTime CreatedAt, AdTouch? Ad)
{
    public static Order Create(
        string productId, string productName, decimal unitPrice, int quantity, AdTouch? ad, DateTime now)
    {
        if (quantity < 1) throw new DomainException("Quantity must be at least 1.");
        return new Order(NewId(), productId, productName, quantity, unitPrice * quantity, now, ad);
    }

    private static string NewId() => $"o-{Guid.NewGuid():N}"[..10];
}
