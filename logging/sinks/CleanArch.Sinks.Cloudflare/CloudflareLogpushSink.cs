using System.Text;
using System.Text.Json;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace CleanArch.Sinks.Cloudflare;

/// <summary>
/// Custom Serilog sink — batches events and POSTs them as
/// compact JSON to Cloudflare Logpush / a Workers logging endpoint.
/// </summary>
public sealed class CloudflareLogpushSink(
    HttpClient httpClient,
    string endpoint) : ILogEventSink
{
    private readonly CompactJsonFormatter _formatter = new();

    public void Emit(LogEvent logEvent)
    {
        var buffer = new StringBuilder();
        using (var writer = new StringWriter(buffer))
            _formatter.Format(logEvent, writer);

        // Fire-and-forget emit — production versions batch on a
        // channel + timer instead of shipping per-event.
        _ = httpClient.PostAsync(endpoint,
            new StringContent(buffer.ToString(), Encoding.UTF8,
                "application/json"));
    }
}
