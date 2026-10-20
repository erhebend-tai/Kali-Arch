using System.Text;
using System.Text.Json;
using Serilog.Core;
using Serilog.Events;

namespace CleanArch.Sinks.Oracle;

/// <summary>
/// Custom Serilog sink — emits events to Oracle Cloud Infrastructure
/// Logging service.
/// </summary>
public sealed class OracleLoggingSink(
    HttpClient httpClient,
    string logId,
    string endpoint) : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        var payload = new
        {
            specversion = "1.0",
            logEntryBatches = new[]
            {
                new
                {
                    entries = new[]
                    {
                        new
                        {
                            data = logEvent.RenderMessage(),
                            id = Guid.NewGuid().ToString(),
                            time = logEvent.Timestamp.UtcDateTime
                        }
                    },
                    source = "CleanArch",
                    type = "com.oraclecloud.logging.custom"
                }
            }
        };

        _ = httpClient.PostAsync($"{endpoint}/logs/{logId}/entries",
            new StringContent(JsonSerializer.Serialize(payload),
                Encoding.UTF8, "application/json"));
    }
}
