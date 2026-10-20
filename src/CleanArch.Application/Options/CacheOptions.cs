using System.ComponentModel.DataAnnotations;

namespace CleanArch.Application.Options;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public string Provider { get; init; } = "Memory"; // Memory | Redis | Hybrid

    public string? RedisConnectionString { get; init; }

    [Range(1, 1440)]
    public int DefaultL1ExpiryMinutes { get; init; } = 1;

    [Range(1, 1440)]
    public int DefaultL2ExpiryMinutes { get; init; } = 30;
}
