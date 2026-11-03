using ClickToCart.Shared.Errors;
using ClickToCart.Shared.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClickToCart.Shared.ServiceDefaults;

/// <summary>Cross-cutting setup every service shares: logging, errors, CORS, Swagger.</summary>
public static class ServiceDefaultsExtensions
{
    private const string CorsPolicy = "AdminUi";

    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder, string serviceName)
    {
        builder.AddFileLogging(serviceName);
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddSwaggerDocs();
        builder.Services.AddAdminUiCors(builder.Configuration);
        builder.Services.AddSingleton(TimeProvider.System);
        return builder;
    }

    public static WebApplication UseServiceDefaults(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseRequestLogging();
        app.UseCors(CorsPolicy);
        app.UseSwaggerDocs();
        return app;
    }

    private static void AddAdminUiCors(this IServiceCollection services, IConfiguration config)
    {
        var origin = config["AdminUi:Origin"] ?? "http://localhost:5173";
        services.AddCors(o => o.AddPolicy(CorsPolicy, p => p.WithOrigins(origin).AllowAnyHeader().AllowAnyMethod()));
    }
}
