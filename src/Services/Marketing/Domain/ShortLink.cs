using ClickToCart.Shared.Errors;

namespace ClickToCart.Marketing.Domain;

public sealed record ProductSnapshot(string Id, string Name, string PageUrl);

/// <summary>One short link per ad creative, so each ad's revenue can be measured separately.</summary>
public sealed class ShortLink
{
    private ShortLink() { }

    public string Code { get; private init; } = "";
    public string CampaignId { get; private init; } = "";
    public string ProductId { get; private init; } = "";
    public string ProductName { get; private init; } = "";
    public string Channel { get; private init; } = "";
    public string AdName { get; private init; } = "";
    public string FinalUrl { get; private init; } = "";
    public DateTime CreatedAt { get; private init; }
    public bool IsEnabled { get; private set; } = true;

    public static ShortLink Create(
        string code, Campaign campaign, ProductSnapshot product, string channel, string adName, DateTime now)
    {
        EnsureCanHaveLinks(campaign);
        var cleanChannel = Guard.NotBlank(channel, "Channel");
        var cleanAd = Guard.NotBlank(adName, "Ad name");
        return new ShortLink
        {
            Code = code,
            CampaignId = campaign.Id,
            ProductId = product.Id,
            ProductName = product.Name,
            Channel = cleanChannel,
            AdName = cleanAd,
            FinalUrl = UtmBuilder.Build(product.PageUrl, cleanChannel, campaign.Name, cleanAd),
            CreatedAt = now
        };
    }

    public void Disable() => IsEnabled = false;

    /// <summary>Paid clicks go to the product only while both the link and its campaign are live.</summary>
    public bool CanRedirect(Campaign campaign) => IsEnabled && campaign.Status == CampaignStatus.Active;

    private static void EnsureCanHaveLinks(Campaign campaign)
    {
        if (campaign.Status != CampaignStatus.Active)
            throw new DomainException("Campaign must be Active before links can be created.");
    }
}
