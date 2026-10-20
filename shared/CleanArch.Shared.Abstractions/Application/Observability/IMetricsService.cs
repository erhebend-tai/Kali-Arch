namespace CleanArch.Shared.Abstractions.Application.Observability;

/// <summary>
/// Custom metrics - counters, histograms, gauges.
/// Implemented by prometheus-net / OpenTelemetry / provider monitors.
/// </summary>
public interface IMetricsService
{
    void IncrementCounter(
        string name,
        IReadOnlyDictionary<string, string>? tags = null);

    void RecordHistogram(
        string name, double value,
        IReadOnlyDictionary<string, string>? tags = null);

    void SetGauge(
        string name, double value,
        IReadOnlyDictionary<string, string>? tags = null);
}
