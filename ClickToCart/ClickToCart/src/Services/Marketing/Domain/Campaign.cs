using ClickToCart.Shared.Errors;

namespace ClickToCart.Marketing.Domain;

public enum CampaignStatus { Draft, Active, Paused, Ended }

/// <summary>Draft -> Active <-> Paused -> Ended. Transitions are enforced here, not in services.</summary>
public sealed class Campaign
{
    public Campaign(string id, string name, string platform, decimal budget, DateTime createdAt,
        CampaignStatus status = CampaignStatus.Draft)
    {
        Id = id;
        Name = name;
        Platform = platform;
        Budget = budget;
        CreatedAt = createdAt;
        Status = status;
    }

    public string Id { get; }
    public string Name { get; }
    public string Platform { get; }
    public decimal Budget { get; }
    public DateTime CreatedAt { get; }
    public CampaignStatus Status { get; private set; }

    public static Campaign Create(string name, string platform, decimal budget, DateTime now)
    {
        if (budget <= 0) throw new DomainException("Budget must be greater than zero.");
        return new Campaign($"c-{Guid.NewGuid():N}"[..10], Guard.NotBlank(name, "Name"),
            Guard.NotBlank(platform, "Platform"), budget, now);
    }

    public void Activate() => MoveTo(CampaignStatus.Active, CampaignStatus.Draft, CampaignStatus.Paused);
    public void Pause() => MoveTo(CampaignStatus.Paused, CampaignStatus.Active);
    public void End() => MoveTo(CampaignStatus.Ended, CampaignStatus.Draft, CampaignStatus.Active, CampaignStatus.Paused);

    private void MoveTo(CampaignStatus target, params CampaignStatus[] allowedFrom)
    {
        if (!allowedFrom.Contains(Status))
            throw new DomainException($"A {Status} campaign cannot become {target}.");
        Status = target;
    }
}
