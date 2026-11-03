using ClickToCart.Marketing.Domain;

namespace ClickToCart.Marketing.Infrastructure;

internal static class MarketingSeed
{
    public static IReadOnlyList<Campaign> Campaigns(DateTime now) =>
    [
        new("c-eid", "Eid Sale", "Facebook", 5000m, now.AddDays(-7), CampaignStatus.Active),
        new("c-school", "Back to School", "Instagram", 3000m, now.AddDays(-5), CampaignStatus.Active),
        new("c-ramadan", "Ramadan Teaser", "Facebook", 2000m, now.AddDays(-1))
    ];

    public static IReadOnlyList<ShortLink> Links(IReadOnlyList<Campaign> campaigns, DateTime now)
    {
        var eid = campaigns[0];
        var school = campaigns[1];
        var airFryer = new ProductSnapshot("p-1", "Air Fryer", "https://shopeg.com/p/air-fryer");
        var phoneCase = new ProductSnapshot("p-2", "Phone Case", "https://shopeg.com/p/phone-case");
        return
        [
            ShortLink.Create("eid7x", eid, airFryer, "facebook", "video_ad", now.AddDays(-6)),
            ShortLink.Create("eid9k", eid, airFryer, "facebook", "image_ad", now.AddDays(-6)),
            ShortLink.Create("case1", school, phoneCase, "instagram", "story_ad", now.AddDays(-4))
        ];
    }
}
