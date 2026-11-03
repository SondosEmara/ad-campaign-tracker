using ClickToCart.Worker.Application;
using ClickToCart.Worker.Domain;
using Microsoft.Extensions.Logging;

namespace ClickToCart.Worker.Infrastructure;

/// <summary>Simulated Meta Marketing API: every click costs a fixed amount.</summary>
public sealed class FakeAdSpendProvider : IAdSpendProvider
{
    private const decimal CostPerClick = 10m;

    public Task<decimal> GetSpendAsync(string linkCode, int clicks, CancellationToken ct) =>
        Task.FromResult(clicks * CostPerClick);
}

/// <summary>Simulated Meta Conversions API: logs the Purchase event it would send.</summary>
public sealed class LoggingMetaConversionsClient(ILogger<LoggingMetaConversionsClient> logger) : IMetaConversionsClient
{
    public Task SendPurchaseAsync(AttributedOrder order, CancellationToken ct)
    {
        logger.LogInformation("[Meta CAPI simulated] Purchase event: order {OrderId}, value {Value} EGP, link {Link}",
            order.OrderId, order.Revenue, order.LinkCode);
        return Task.CompletedTask;
    }
}
