using System.Text.RegularExpressions;

namespace ClickToCart.Marketing.Domain;

/// <summary>Pure functions that append UTM tags so analytics tools can see the ad source.</summary>
public static partial class UtmBuilder
{
    public static string Build(string pageUrl, string channel, string campaign, string ad)
    {
        var tags = new Dictionary<string, string>
        {
            ["utm_source"] = Slug(channel),
            ["utm_medium"] = "paid",
            ["utm_campaign"] = Slug(campaign),
            ["utm_content"] = Slug(ad)
        };
        var separator = pageUrl.Contains('?') ? '&' : '?';
        return pageUrl + separator + string.Join('&', tags.Select(t => $"{t.Key}={t.Value}"));
    }

    public static string Slug(string value) =>
        NonAlphaNumeric().Replace(value.Trim().ToLowerInvariant(), "_").Trim('_');

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonAlphaNumeric();
}
