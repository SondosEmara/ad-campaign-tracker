using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClickToCart.Shared.Errors;

public class DomainException(string message) : Exception(message);

public sealed class NotFoundException(string entity, string key)
    : DomainException($"{entity} '{key}' was not found.");

public static class Guard
{
    public static string NotBlank(string? value, string field) =>
        string.IsNullOrWhiteSpace(value) ? throw new DomainException($"{field} is required.") : value.Trim();
}

/// <summary>Turns exceptions into consistent JSON problem responses and logs them.</summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var status = StatusFor(exception);
        Log(exception, status);
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(ToProblem(exception, status), ct);
        return true;
    }

    private static int StatusFor(Exception exception) => exception switch
    {
        NotFoundException => StatusCodes.Status404NotFound,
        DomainException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };

    private static ProblemDetails ToProblem(Exception exception, int status) => new()
    {
        Status = status,
        Title = status == StatusCodes.Status500InternalServerError ? "Unexpected error" : exception.Message
    };

    private void Log(Exception exception, int status)
    {
        if (status >= 500) logger.LogError(exception, "Unhandled exception");
        else logger.LogWarning("Request rejected: {Message}", exception.Message);
    }
}
