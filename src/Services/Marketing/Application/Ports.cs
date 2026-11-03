using ClickToCart.Marketing.Domain;
using ClickToCart.Shared.Contracts;

namespace ClickToCart.Marketing.Application;

public interface ICampaignRepository
{
    Task AddAsync(Campaign campaign, CancellationToken ct);
    Task<Campaign?> FindAsync(string id, CancellationToken ct);
    Task<IReadOnlyList<Campaign>> ListAsync(CancellationToken ct);
    Task SaveAsync(Campaign campaign, CancellationToken ct);
}

public interface ILinkRepository
{
    Task AddAsync(ShortLink link, CancellationToken ct);
    Task<ShortLink?> FindAsync(string code, CancellationToken ct);
    Task<IReadOnlyList<ShortLink>> ListAsync(CancellationToken ct);
    Task<bool> ExistsAsync(string code, CancellationToken ct);
    Task SaveAsync(ShortLink link, CancellationToken ct);
}

public interface ICodeGenerator
{
    string Next();
}

/// <summary>Port to the Products microservice.</summary>
public interface IProductCatalog
{
    Task<ProductDto> GetAsync(string productId, CancellationToken ct);
}

/// <summary>Port to the Worker's metrics API (clicks, attributed orders, ad spend).</summary>
public interface IMetricsClient
{
    Task<IReadOnlyList<LinkMetricsDto>> GetLinkMetricsAsync(CancellationToken ct);
}

public interface ICampaignService
{
    Task<IReadOnlyList<CampaignDto>> ListAsync(CancellationToken ct);
    Task<CampaignDto> CreateAsync(CreateCampaignRequest request, CancellationToken ct);
    Task<CampaignDto> ChangeStatusAsync(string id, string action, CancellationToken ct);
}

public interface ILinkService
{
    Task<IReadOnlyList<LinkDto>> ListAsync(CancellationToken ct);
    Task<LinkDto> CreateAsync(CreateLinkRequest request, CancellationToken ct);
    Task<LinkDto> DisableAsync(string code, CancellationToken ct);
}

public interface IRedirectService
{
    Task<RedirectResult> ResolveAsync(ClickContext click, CancellationToken ct);
}

public interface IReportingService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken ct);
}
