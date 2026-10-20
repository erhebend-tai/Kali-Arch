using System.ComponentModel.DataAnnotations;

namespace CleanArch.Application.Options;

public sealed class ComputeOptions
{
    public const string SectionName = "Compute";

    [Range(1, 1000)]
    public int MaxVmsPerUser { get; init; } = 10;

    [Range(1, 100)]
    public int MaxVmsPerResourceGroup { get; init; } = 50;

    [Required]
    public string DefaultRegion { get; init; } = "us-east";

    [Required]
    public string DefaultSize { get; init; } = "medium";

    public bool AutoShutdownEnabled { get; init; } = true;

    [Range(1, 24)]
    public int AutoShutdownAfterHours { get; init; } = 8;

    public IReadOnlyList<string> AllowedRegions { get; init; } =
        ["us-east", "us-west", "europe-west", "asia-east"];
}
