using System.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Memory;

/// <summary>
/// Periodic GC metrics — drives the
/// container_memory_working_set_bytes dashboard panel.
/// </summary>
public sealed class MemoryPressureMonitor(
    ILogger<MemoryPressureMonitor> logger)
    : BackgroundService
{
    private static readonly Meter MemoryMeter =
        new("CleanArch.Memory");

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        MemoryMeter.CreateObservableGauge(
            "memory_working_set_bytes",
            () => Environment.WorkingSet);

        while (!stoppingToken.IsCancellationRequested)
        {
            var info = GC.GetGCMemoryInfo();
            var pressure = (double)info.MemoryLoadBytes /
                info.TotalAvailableMemoryBytes;

            if (pressure > 0.85)
            {
                logger.LogWarning(
                    "Memory pressure high: {Pressure:P0}. " +
                    "Gen2 collections: {Gen2}",
                    pressure,
                    GC.CollectionCount(2));

                GC.Collect(2, GCCollectionMode.Aggressive,
                    blocking: false);
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
