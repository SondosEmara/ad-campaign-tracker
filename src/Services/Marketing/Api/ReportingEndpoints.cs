using ClickToCart.Marketing.Application;

namespace ClickToCart.Marketing.Api;

public static class ReportingEndpoints
{
    public static IEndpointRouteBuilder MapReportingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard", (IReportingService s, CancellationToken ct) => s.GetDashboardAsync(ct))
            .WithTags("Reporting");
        return app;
    }
}
