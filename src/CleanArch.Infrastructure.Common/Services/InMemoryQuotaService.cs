using System.Collections.Concurrent;
using CleanArch.Domain.Interfaces;

namespace CleanArch.Infrastructure.Common.Services;

/// <summary>
/// Dev/test quota fallback — tracks counts per region+size in memory.
/// Real providers replace this with ARM/Compute quota APIs.
/// </summary>
public sealed class InMemoryQuotaService : IQuotaService
{
    private const int MaxPerRegionAndSize = 100;
    private readonly ConcurrentDictionary<string, int> _used = new();

    public Task<bool> HasAvailableQuotaAsync(
        string region, string size, CancellationToken ct = default)
    {
        var key = $"{region}:{size}";
        var used = _used.GetOrAdd(key, 0);
        return Task.FromResult(used < MaxPerRegionAndSize);
    }
}
