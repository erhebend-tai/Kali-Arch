using System.Reactive.Linq;

namespace CleanArch.Application.Reactive.Streams;

/// <summary>
/// Reactive metrics pipeline — rolling averages → sustained-alert
/// detection. (R3 alternative: swap IObservable for R3.Observable.)
/// </summary>
public sealed class MetricsPipeline(IMetricsCollector collector)
{
    public IObservable<MetricAlert> BuildAlertPipeline() =>
        collector.MetricEvents
            .Where(m => m.Name == "cpu_usage")
            .GroupBy(m => m.VmId)
            .SelectMany(group =>
                group
                    .Buffer(TimeSpan.FromSeconds(60),
                            TimeSpan.FromSeconds(10))
                    .Where(window => window.Count > 0)
                    .Select(window => new
                    {
                        VmId = group.Key,
                        Average = window.Average(m => m.Value)
                    })
                    .Where(avg => avg.Average > 90.0)
                    .Select(avg => new MetricAlert(
                        avg.VmId,
                        "HighCpuUsage",
                        $"CPU averaged {avg.Average:F1}% over 60s",
                        AlertSeverity.Warning)));
}

public interface IMetricsCollector
{
    IObservable<MetricEvent> MetricEvents { get; }
}

public sealed record MetricEvent(
    Guid VmId,
    string Name,
    double Value,
    DateTime ObservedAt);

public sealed record MetricAlert(
    Guid VmId,
    string AlertType,
    string Message,
    AlertSeverity Severity);

public enum AlertSeverity { Info, Warning, Critical }
