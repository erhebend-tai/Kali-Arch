using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CleanArch.Infrastructure.Common.Observability;

/// <summary>
/// Central ActivitySource + Meter — every component traces and
/// counts through these, and exporters pick them up by name.
/// </summary>
public static class Diagnostics
{
    public const string ServiceName = "CleanArch";
    public const string ServiceVersion = "1.0.0";

    public static readonly ActivitySource ActivitySource =
        new(ServiceName, ServiceVersion);

    public static readonly Meter Meter =
        new(ServiceName, ServiceVersion);

    public static readonly Counter<long> RequestsTotal =
        Meter.CreateCounter<long>("platform_requests_total",
            description: "Total processed requests");

    public static readonly Counter<long> ErrorsTotal =
        Meter.CreateCounter<long>("platform_errors_total",
            description: "Total unhandled errors");

    public static readonly Histogram<double> RequestDuration =
        Meter.CreateHistogram<double>(
            "platform_request_duration_ms",
            unit: "ms",
            description: "Request duration in milliseconds");
}
