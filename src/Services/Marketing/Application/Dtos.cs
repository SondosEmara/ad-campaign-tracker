using ClickToCart.Marketing.Domain;

namespace ClickToCart.Marketing.Application;

public sealed class ShortLinkOptions
{
    public string BaseUrl { get; set; } = "http://localhost:5103/r";
    public string FallbackUrl { get; set; } = "https://shopeg.com";
}

public sealed record CampaignDto(string Id, string Name, string Platform, decimal Budget, string Status, DateTime CreatedAt);
public sealed record CreateCampaignRequest(string Name, string Platform, decimal Budget);

public sealed record LinkDto(
    string Code, string ShortUrl, string CampaignId, string ProductId, string ProductName,
    string Channel, string AdName, string FinalUrl, bool IsEnabled, DateTime CreatedAt);
public sealed record CreateLinkRequest(string CampaignId, string ProductId, string Channel, string AdName);

public sealed record ClickContext(string Code, string? VisitorId, string? Fbclid);
public sealed record RedirectResult(string TargetUrl, string VisitorId);
public sealed record SimulateClickRequest(string Code, string? VisitorId);

public sealed record LinkPerformanceDto(
    string Code, string CampaignName, string ProductName, string Channel, string AdName,
    int Clicks, int Orders, decimal Revenue, decimal Spend, decimal Roas, decimal Cpa, decimal ConversionRate);
public sealed record DashboardTotalsDto(
    int Clicks, int Orders, decimal Revenue, decimal Spend, decimal Roas, decimal Cpa, decimal ConversionRate);
public sealed record DashboardDto(DashboardTotalsDto Totals, IReadOnlyList<LinkPerformanceDto> Links);

public static class Mappers
{
    public static CampaignDto ToDto(Campaign c) => new(c.Id, c.Name, c.Platform, c.Budget, c.Status.ToString(), c.CreatedAt);

    public static LinkDto ToDto(ShortLink l, string baseUrl) => new(
        l.Code, $"{baseUrl.TrimEnd('/')}/{l.Code}", l.CampaignId, l.ProductId, l.ProductName,
        l.Channel, l.AdName, l.FinalUrl, l.IsEnabled, l.CreatedAt);
}
