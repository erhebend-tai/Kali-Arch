using System.Text;
using System.Text.Json;
using Serilog.Core;
using Serilog.Events;

namespace CleanArch.Sinks.Loki;

/// <summary>
/// Custom Serilog sink — pushes events to a Grafana Loki
/// /loki/api/v1/push endpoint.
/// </summary>
public sealed class LokiSink(
    HttpClient httpClient,
    string lokiUrl,
    string serviceName) : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        var labels = new Dictionary<string, string>
        {
            ["service"] = serviceName,
            ["level"] = logEvent.Level.ToString().ToLowerInvariant()
        };

        var timestampNs =
            (logEvent.Timestamp.UtcDateTime.Ticks -
             DateTime.UnixEpoch.Ticks) * 100L;

        var payload = new
        {
            streams = new[]
            {
                new
                {
                    stream = labels,
                    values = new[]
                    {
                        new[] { timestampNs.ToString(),
                                logEvent.RenderMessage() }
                    }
                }
            }
        };

        _ = httpClient.PostAsync($"{lokiUrl}/loki/api/v1/push",
            new StringContent(JsonSerializer.Serialize(payload),
                Encoding.UTF8, "application/json"));
    }
}
