using Serilog.Core;
using Serilog.Events;

namespace CleanArch.Logging.Enrichers;

public sealed class CloudProviderEnricher(string cloudProvider)
    : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
    {
        logEvent.AddPropertyIfAbsent(
            factory.CreateProperty("CloudProvider", cloudProvider));
    }
}
