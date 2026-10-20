using System.Text.Json;
using CleanArch.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArch.Infrastructure.Common.Cache;

/// <summary>
/// L1 — in-process MemoryCache. Single-node deployments or
/// first tier of the hybrid cache.
/// </summary>
public sealed class MemoryCacheService(IMemoryCache cache)
    : ICacheService
{
    private static readonly TimeSpan DefaultExpiry =
        TimeSpan.FromMinutes(10);

    public Task<T?> GetAsync<T>(
        string key, CancellationToken ct = default)
    {
        if (cache.TryGetValue(key, out string? json) && json is not null)
            return Task.FromResult(
                JsonSerializer.Deserialize<T>(json));

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(
        string key, T value, TimeSpan? expiry = null,
        CancellationToken ct = default)
    {
        cache.Set(key, JsonSerializer.Serialize(value),
            expiry ?? DefaultExpiry);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }

    /// <summary>
    /// MemoryCache can't scan keys — pattern removal needs the
    /// Redis or Hybrid implementation.
    /// </summary>
    public Task RemoveByPatternAsync(
        string pattern, CancellationToken ct = default) =>
        Task.CompletedTask;

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken ct = default)
    {
        var cached = await GetAsync<T>(key, ct);
        if (cached is not null)
            return cached;

        var value = await factory(ct);
        await SetAsync(key, value, expiry, ct);
        return value;
    }
}
