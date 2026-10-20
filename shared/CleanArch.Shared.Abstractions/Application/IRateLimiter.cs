using CleanArch.Shared.Abstractions.DTOs.Common;

namespace CleanArch.Shared.Abstractions.Application;

public interface IRateLimiter
{
    Task<RateLimitResult> CheckAsync(
        string key,
        int limit,
        TimeSpan window,
        CancellationToken ct = default);
}
