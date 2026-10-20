using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CleanArch.Shared.Infrastructure.Middleware;

/// <summary>
/// Reads (or mints) an X-Correlation-Id and pushes it into
/// the response headers + log scope.
/// </summary>
public sealed class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId =
            context.Request.Headers[HeaderName].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object>
               { ["CorrelationId"] = correlationId }))
        {
            await next(context);
        }
    }
}
