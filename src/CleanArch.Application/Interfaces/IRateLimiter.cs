namespace CleanArch.Application.Interfaces;

public interface IRateLimiter
{
    Task<RateLimitResult> CheckAsync(
        string key,
        int limit,
        TimeSpan window,
        CancellationToken ct = default);
}

public sealed record RateLimitResult(
    bool Allowed,
    int Remaining,
    int Limit,
    string? RetryAfter);
