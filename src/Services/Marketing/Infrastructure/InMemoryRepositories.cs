using System.Collections.Concurrent;
using ClickToCart.Marketing.Application;
using ClickToCart.Marketing.Domain;

namespace ClickToCart.Marketing.Infrastructure;

public sealed class InMemoryCampaignRepository(TimeProvider clock) : ICampaignRepository
{
    private readonly ConcurrentDictionary<string, Campaign> _items = new(
        MarketingSeed.Campaigns(clock.GetUtcNow().UtcDateTime).ToDictionary(c => c.Id));

    public Task AddAsync(Campaign campaign, CancellationToken ct)
    {
        _items[campaign.Id] = campaign;
        return Task.CompletedTask;
    }

    public Task<Campaign?> FindAsync(string id, CancellationToken ct) =>
        Task.FromResult(_items.GetValueOrDefault(id));

    public Task<IReadOnlyList<Campaign>> ListAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Campaign>>(_items.Values.ToList());

    public Task SaveAsync(Campaign campaign, CancellationToken ct) => AddAsync(campaign, ct);
}

public sealed class InMemoryLinkRepository(ICampaignRepository campaigns, TimeProvider clock) : ILinkRepository
{
    private readonly ConcurrentDictionary<string, ShortLink> _items = new(
        MarketingSeed.Links(campaigns.ListAsync(CancellationToken.None).Result.OrderBy(c => c.CreatedAt).ToList(),
            clock.GetUtcNow().UtcDateTime).ToDictionary(l => l.Code));

    public Task AddAsync(ShortLink link, CancellationToken ct)
    {
        _items[link.Code] = link;
        return Task.CompletedTask;
    }

    public Task<ShortLink?> FindAsync(string code, CancellationToken ct) =>
        Task.FromResult(_items.GetValueOrDefault(code));

    public Task<IReadOnlyList<ShortLink>> ListAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<ShortLink>>(_items.Values.ToList());

    public Task<bool> ExistsAsync(string code, CancellationToken ct) =>
        Task.FromResult(_items.ContainsKey(code));

    public Task SaveAsync(ShortLink link, CancellationToken ct) => AddAsync(link, ct);
}
