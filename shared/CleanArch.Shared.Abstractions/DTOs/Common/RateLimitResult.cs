namespace CleanArch.Shared.Abstractions.DTOs.Common;

public sealed record RateLimitResult(
    bool Allowed,
    int Remaining,
    int Limit,
    string? RetryAfter);
