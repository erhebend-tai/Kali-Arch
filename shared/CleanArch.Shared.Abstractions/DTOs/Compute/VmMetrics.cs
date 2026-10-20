namespace CleanArch.Shared.Abstractions.DTOs.Compute;

/// <summary>Point-in-time VM utilization metrics.</summary>
public sealed record VmMetrics(
    string VmId,
    double CpuPercent,
    double MemoryPercent,
    double DiskReadBytesPerSec,
    double DiskWriteBytesPerSec,
    double NetworkInBytesPerSec,
    double NetworkOutBytesPerSec,
    DateTime CollectedAt);
