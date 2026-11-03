using ClickToCart.Marketing.Domain;
using ClickToCart.Shared.Contracts;
using Microsoft.Extensions.Logging;

namespace ClickToCart.Marketing.Application;

public sealed class ReportingService(
    ILinkRepository links,
    ICampaignRepository campaigns,
    IMetricsClient metricsClient,
    ILogger<ReportingService> logger) : IReportingService
{
    public async Task<DashboardDto> GetDashboardAsync(CancellationToken ct)
    {
        var metrics = await LoadMetricsAsync(ct);
        var campaignNames = (await campaigns.ListAsync(ct)).ToDictionary(c => c.Id, c => c.Name);
        var rows = (await links.ListAsync(ct))
            .Select(link => ToRow(link, campaignNames, metrics.GetValueOrDefault(link.Code) ?? LinkMetricsDto.Empty(link.Code)))
            .OrderByDescending(r => r.Revenue)
            .ToList();
        logger.LogInformation("Dashboard built for {Count} links", rows.Count);
        return new DashboardDto(ToTotals(rows), rows);
    }

    private async Task<Dictionary<string, LinkMetricsDto>> LoadMetricsAsync(CancellationToken ct)
    {
        try
        {
            return (await metricsClient.GetLinkMetricsAsync(ct)).ToDictionary(m => m.LinkCode);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Worker metrics unavailable; showing zeros");
            return [];
        }
    }

    private static LinkPerformanceDto ToRow(ShortLink link, Dictionary<string, string> campaignNames, LinkMetricsDto m) => new(
        link.Code, campaignNames.GetValueOrDefault(link.CampaignId, "Unknown"), link.ProductName, link.Channel, link.AdName,
        m.Clicks, m.Orders, m.Revenue, m.Spend,
        Kpi.Roas(m.Revenue, m.Spend), Kpi.Cpa(m.Spend, m.Orders), Kpi.ConversionRate(m.Orders, m.Clicks));

    private static DashboardTotalsDto ToTotals(IReadOnlyList<LinkPerformanceDto> rows)
    {
        var clicks = rows.Sum(r => r.Clicks);
        var orders = rows.Sum(r => r.Orders);
        var revenue = rows.Sum(r => r.Revenue);
        var spend = rows.Sum(r => r.Spend);
        return new DashboardTotalsDto(clicks, orders, revenue, spend,
            Kpi.Roas(revenue, spend), Kpi.Cpa(spend, orders), Kpi.ConversionRate(orders, clicks));
    }
}
