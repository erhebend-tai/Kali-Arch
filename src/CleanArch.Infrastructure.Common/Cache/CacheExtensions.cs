using CleanArch.Application.Interfaces;
using CleanArch.Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CleanArch.Infrastructure.Common.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration config)
    {
        var options = config
            .GetSection(CacheOptions.SectionName)
            .Get<CacheOptions>() ?? new CacheOptions();

        services.AddMemoryCache();

        switch (options.Provider.ToLowerInvariant())
        {
            case "redis":
                services.AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(
                        options.RedisConnectionString
                        ?? "localhost:6379"));
                services.AddSingleton<ICacheService, RedisCacheService>();
                break;

            case "hybrid":
                services.AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(
                        options.RedisConnectionString
                        ?? "localhost:6379"));
                services.AddHybridCache();
                services.AddSingleton<ICacheService, HybridCacheService>();
                break;

            default:
                services.AddSingleton<ICacheService, MemoryCacheService>();
                break;
        }

        return services;
    }
}
