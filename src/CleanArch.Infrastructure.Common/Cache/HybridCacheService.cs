using CleanArch.Application.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;

namespace CleanArch.Infrastructure.Common.Cache;

/// <summary>
/// L1 (Memory) + L2 (Redis) with stampede protection —
/// Microsoft.Extensions.Caching.Hybrid.
/// </summary>
public sealed class HybridCacheService(HybridCache cache)
    : ICacheService
{
    public async Task<T?> GetAsync<T>(
        string key, CancellationToken ct = default) =>
        await cache.GetOrCreateAsync<T?>(
            key,
            _ => ValueTask.FromResult<T?>(default),
            cancellationToken: ct);

    public async Task SetAsync<T>(
        string key, T value, TimeSpan? expiry = null,
        CancellationToken ct = default)
    {
        var options = expiry.HasValue
            ? new HybridCacheEntryOptions { Expiration = expiry }
            : null;
        await cache.SetAsync(key, value, options,
            cancellationToken: ct);
    }

    public async Task RemoveAsync(
        string key, CancellationToken ct = default) =>
        await cache.RemoveAsync(key, ct);

    public Task RemoveByPatternAsync(
        string pattern, CancellationToken ct = default) =>
        Task.CompletedTask; // HybridCache doesn't support key scanning

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken ct = default) =>
        await cache.GetOrCreateAsync(
            key,
            async c => await factory(c),
            new HybridCacheEntryOptions { Expiration = expiry },
            cancellationToken: ct);
}
