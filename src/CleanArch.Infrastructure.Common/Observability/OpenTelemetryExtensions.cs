using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure.Common.Observability;

/// <summary>
/// One-call wiring — traces + metrics through OTLP,
/// instrumentation for ASP.NET Core, HttpClient, runtime.
/// </summary>
public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddPlatformObservability(
        this IServiceCollection services,
        string? otlpEndpoint = null)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(Diagnostics.ServiceName,
                    serviceVersion: Diagnostics.ServiceVersion))
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(Diagnostics.ServiceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (otlpEndpoint is not null)
                    tracing.AddOtlpExporter(o =>
                        o.Endpoint = new Uri(otlpEndpoint));
                else
                    tracing.AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(Diagnostics.ServiceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (otlpEndpoint is not null)
                    metrics.AddOtlpExporter(o =>
                        o.Endpoint = new Uri(otlpEndpoint));
                else
                    metrics.AddConsoleExporter();
            });

        return services;
    }
}
