using System.Threading;
using CleanArch.Application.Interfaces;

namespace CleanArch.Infrastructure.Common.Services;

/// <summary>
/// In-memory metrics fallback. Production deployments replace this
/// with a SignalR/TimeSeries-backed implementation.
/// </summary>
public sealed class InMemoryDashboardService : IDashboardService
{
    private int _vmCount;

    public int VmCount => _vmCount;

    public Task IncrementVmCountAsync(CancellationToken ct = default)
    {
        Interlocked.Increment(ref _vmCount);
        return Task.CompletedTask;
    }

    public Task DecrementVmCountAsync(CancellationToken ct = default)
    {
        Interlocked.Decrement(ref _vmCount);
        return Task.CompletedTask;
    }
}
