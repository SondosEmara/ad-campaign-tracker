using ClickToCart.Marketing.Domain;
using ClickToCart.Shared.Contracts;
using ClickToCart.Shared.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClickToCart.Marketing.Application;

public sealed class LinkService(
    ILinkRepository links,
    ICampaignRepository campaigns,
    IProductCatalog catalog,
    ICodeGenerator codes,
    IOptions<ShortLinkOptions> options,
    TimeProvider clock,
    ILogger<LinkService> logger) : ILinkService
{
    private const int MaxCodeAttempts = 5;
    private string BaseUrl => options.Value.BaseUrl;

    public async Task<IReadOnlyList<LinkDto>> ListAsync(CancellationToken ct)
    {
        var all = await links.ListAsync(ct);
        return all.OrderByDescending(l => l.CreatedAt).Select(l => Mappers.ToDto(l, BaseUrl)).ToList();
    }

    public async Task<LinkDto> CreateAsync(CreateLinkRequest request, CancellationToken ct)
    {
        var campaign = await campaigns.FindAsync(request.CampaignId, ct)
            ?? throw new NotFoundException("Campaign", request.CampaignId);
        var product = await catalog.GetAsync(request.ProductId, ct);

        var link = ShortLink.Create(await NextFreeCodeAsync(ct), campaign, ToSnapshot(product),
            request.Channel, request.AdName, clock.GetUtcNow().UtcDateTime);


        await links.AddAsync(link, ct);
        logger.LogInformation("Link {Code} created: campaign {Campaign}, product {Product}, ad {Ad}",
            link.Code, campaign.Name, link.ProductName, link.AdName);
        return Mappers.ToDto(link, BaseUrl);
    }

    public async Task<LinkDto> DisableAsync(string code, CancellationToken ct)
    {
        var link = await links.FindAsync(code, ct) ?? throw new NotFoundException("Link", code);
        link.Disable();
        await links.SaveAsync(link, ct);
        logger.LogInformation("Link {Code} disabled", code);
        return Mappers.ToDto(link, BaseUrl);
    }

    private static ProductSnapshot ToSnapshot(ProductDto p) => new(p.Id, p.Name, p.PageUrl);

    private async Task<string> NextFreeCodeAsync(CancellationToken ct)
    {
        for (var attempt = 0; attempt < MaxCodeAttempts; attempt++)
        {
            var code = codes.Next();
            if (!await links.ExistsAsync(code, ct)) return code;
        }
        throw new DomainException("Could not generate a unique short code. Please try again.");
    }
}
