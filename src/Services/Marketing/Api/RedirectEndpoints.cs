using ClickToCart.Marketing.Application;

namespace ClickToCart.Marketing.Api;

/// <summary>Public click handling. Kept thin so it can be split into its own service later.</summary>
public static class RedirectEndpoints
{
    private const string CookieName = "ad_touch";
    private static readonly TimeSpan CookieLifetime = TimeSpan.FromDays(7);

    public static IEndpointRouteBuilder MapRedirectEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/r/{code}", RedirectAsync).WithTags("Redirect");
        app.MapPost("/api/simulator/click", SimulateClickAsync).WithTags("Simulator");
        return app;
    }

    private static async Task<IResult> RedirectAsync(
        string code, HttpContext http, IRedirectService redirects, CancellationToken ct)
    {
        var click = new ClickContext(code, http.Request.Cookies[CookieName], http.Request.Query["fbclid"].FirstOrDefault());
        var result = await redirects.ResolveAsync(click, ct);
        AppendCookie(http, result.VisitorId);
        return Results.Redirect(result.TargetUrl);
    }

    private static Task<RedirectResult> SimulateClickAsync(
        SimulateClickRequest request, IRedirectService redirects, CancellationToken ct) =>
        redirects.ResolveAsync(new ClickContext(request.Code, request.VisitorId, null), ct);

    private static void AppendCookie(HttpContext http, string visitorId) =>
        http.Response.Cookies.Append(CookieName, visitorId, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.Add(CookieLifetime),
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        });
}
