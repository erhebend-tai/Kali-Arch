using System.Text.Json;
using CleanArch.Application.Interfaces;
using StackExchange.Redis;

namespace CleanArch.Infrastructure.Common.Cache;

/// <summary>L2 — distributed Redis. Shared across replicas.</summary>
public sealed class RedisCacheService(IConnectionMultiplexer redis)
    : ICacheService
{
    private static readonly TimeSpan DefaultExpiry =
        TimeSpan.FromMinutes(30);

    private IDatabase Db => redis.GetDatabase();

    public async Task<T?> GetAsync<T>(
        string key, CancellationToken ct = default)
    {
        var value = await Db.StringGetAsync(key);
        return value.IsNullOrEmpty
            ? default
            : JsonSerializer.Deserialize<T>((string)value!);
    }

    public async Task SetAsync<T>(
        string key, T value, TimeSpan? expiry = null,
        CancellationToken ct = default) =>
        await Db.StringSetAsync(
            key, JsonSerializer.Serialize(value),
            expiry ?? DefaultExpiry);

    public async Task RemoveAsync(
        string key, CancellationToken ct = default) =>
        await Db.KeyDeleteAsync(key);

    public async Task RemoveByPatternAsync(
        string pattern, CancellationToken ct = default)
    {
        var endpoints = redis.GetEndPoints();
        var server = redis.GetServer(endpoints[0]);

        var keys = server.Keys(pattern: pattern).ToArray();
        if (keys.Length > 0)
            await Db.KeyDeleteAsync(keys);
    }

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
