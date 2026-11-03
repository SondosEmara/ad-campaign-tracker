using ClickToCart.Marketing.Domain;
using ClickToCart.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace ClickToCart.Marketing.Application;

public sealed class CampaignService(
    ICampaignRepository repository, TimeProvider clock, ILogger<CampaignService> logger) : ICampaignService
{
    private static readonly Dictionary<string, Action<Campaign>> StatusActions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["activate"] = c => c.Activate(),
            ["pause"] = c => c.Pause(),
            ["end"] = c => c.End()
        };

    public async Task<IReadOnlyList<CampaignDto>> ListAsync(CancellationToken ct)
    {
        var campaigns = await repository.ListAsync(ct);
        return campaigns.OrderByDescending(c => c.CreatedAt).Select(Mappers.ToDto).ToList();
    }

    public async Task<CampaignDto> CreateAsync(CreateCampaignRequest request, CancellationToken ct)
    {
        var campaign = Campaign.Create(request.Name, request.Platform, request.Budget, clock.GetUtcNow().UtcDateTime);
        await repository.AddAsync(campaign, ct);
        logger.LogInformation("Campaign {CampaignId} '{Name}' created with budget {Budget}", campaign.Id, campaign.Name, campaign.Budget);
        return Mappers.ToDto(campaign);
    }

    public async Task<CampaignDto> ChangeStatusAsync(string id, string action, CancellationToken ct)
    {
        var campaign = await repository.FindAsync(id, ct) ?? throw new NotFoundException("Campaign", id);
        ApplyAction(campaign, action);
        await repository.SaveAsync(campaign, ct);
        logger.LogInformation("Campaign {CampaignId} is now {Status}", id, campaign.Status);
        return Mappers.ToDto(campaign);
    }

    private static void ApplyAction(Campaign campaign, string action)
    {
        if (!StatusActions.TryGetValue(action, out var apply))
            throw new DomainException($"Unknown action '{action}'. Use activate, pause or end.");
        apply(campaign);
    }
}
