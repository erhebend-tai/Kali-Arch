using CleanArch.Logging.Enrichers;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Compact;

namespace CleanArch.Logging.Configuration;

/// <summary>
/// Central Serilog configuration — every entry point calls this.
/// Sinks, enrichers, minimum level all live here so API, Functions,
/// workers, and CLIs emit identical structured logs.
/// </summary>
public static class LoggingConfiguration
{
    public static LoggerConfiguration Create(
        IConfiguration configuration,
        string serviceName)
    {
        var cloudProvider =
            configuration.GetValue<string>("CloudProvider") ?? "Unknown";

        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.With(new ServiceNameEnricher(serviceName))
            .Enrich.With(new CloudProviderEnricher(cloudProvider))
            .Enrich.With(new CorrelationIdEnricher())
            .Enrich.WithProperty("Application", "CleanArch")
            .WriteTo.Console(new CompactJsonFormatter());
    }
}
