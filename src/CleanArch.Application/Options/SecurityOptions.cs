using System.ComponentModel.DataAnnotations;

namespace CleanArch.Application.Options;

public sealed class SecurityOptions
{
    public const string SectionName = "Security";

    public int RateLimitPerMinute { get; init; } = 1000;
    public int ProvisionRateLimitPerMinute { get; init; } = 10;

    public bool RequireAuth { get; init; } = true;

    [Range(1, 90)]
    public int SecretRotationBufferDays { get; init; } = 7;
}
