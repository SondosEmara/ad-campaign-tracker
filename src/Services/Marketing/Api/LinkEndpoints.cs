using ClickToCart.Marketing.Application;

namespace ClickToCart.Marketing.Api;

public static class LinkEndpoints
{
    public static IEndpointRouteBuilder MapLinkEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/links").WithTags("Links");
        group.MapGet("/", (ILinkService s, CancellationToken ct) => s.ListAsync(ct));
        group.MapPost("/", CreateAsync);
        group.MapPost("/{code}/disable", (string code, ILinkService s, CancellationToken ct) => s.DisableAsync(code, ct));
        return app;
    }

    private static async Task<IResult> CreateAsync(CreateLinkRequest request, ILinkService service, CancellationToken ct)
    {
        var link = await service.CreateAsync(request, ct);
        return Results.Created($"/api/links/{link.Code}", link);
    }
}
