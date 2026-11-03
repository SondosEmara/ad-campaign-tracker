using ClickToCart.Marketing.Application;

namespace ClickToCart.Marketing.Api;

public static class CampaignEndpoints
{
    public static IEndpointRouteBuilder MapCampaignEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/campaigns").WithTags("Campaigns");
        group.MapGet("/", (ICampaignService s, CancellationToken ct) => s.ListAsync(ct));
        group.MapPost("/", CreateAsync);
        group.MapPost("/{id}/status/{action}", (string id, string action, ICampaignService s, CancellationToken ct) =>
            s.ChangeStatusAsync(id, action, ct));
        return app;
    }

    private static async Task<IResult> CreateAsync(CreateCampaignRequest request, ICampaignService service, CancellationToken ct)
    {
        var campaign = await service.CreateAsync(request, ct);
        return Results.Created($"/api/campaigns/{campaign.Id}", campaign);
    }
}
