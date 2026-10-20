using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace CleanArch.Logging.Enrichers;

/// <summary>
/// Copies X-Correlation-Id from the current HttpContext onto
/// every log event. Outside HTTP (workers/functions) it no-ops —
/// entry points push correlation ids via LogContext themselves.
/// </summary>
public sealed class CorrelationIdEnricher : ILogEventEnricher
{
    private static readonly AsyncLocal<HttpContext?> CurrentContext = new();
    private static IHttpContextAccessor? _accessor;

    public static void Configure(IHttpContextAccessor accessor) =>
        _accessor = accessor;

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
    {
        var correlationId =
            _accessor?.HttpContext?.Items["CorrelationId"]?.ToString();

        if (correlationId is not null)
            logEvent.AddPropertyIfAbsent(
                factory.CreateProperty("CorrelationId", correlationId));
    }
}
