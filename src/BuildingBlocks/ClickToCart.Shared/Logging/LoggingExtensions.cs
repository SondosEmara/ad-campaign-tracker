using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace ClickToCart.Shared.Logging;

public static class LoggingExtensions
{
    private const string Template = "{Timestamp:HH:mm:ss} [{Level:u3}] {Service} | {Message:lj}{NewLine}{Exception}";

    /// <summary>Console + rolling daily file logs in ./logs/{service}-yyyyMMdd.log</summary>
    public static WebApplicationBuilder AddFileLogging(this WebApplicationBuilder builder, string serviceName)
    {
        builder.Host.UseSerilog((_, config) => config
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http", LogEventLevel.Warning)
            .Enrich.WithProperty("Service", serviceName)
            .WriteTo.Console(outputTemplate: Template)
            .WriteTo.File($"logs/{serviceName}-.log", rollingInterval: RollingInterval.Day, outputTemplate: Template));
        return builder;
    }

    public static WebApplication UseRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}
