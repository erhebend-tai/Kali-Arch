using Serilog.Core;
using Serilog.Events;

namespace CleanArch.Logging.Enrichers;

public sealed class ServiceNameEnricher(string serviceName) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
    {
        logEvent.AddPropertyIfAbsent(
            factory.CreateProperty("ServiceName", serviceName));
    }
}
