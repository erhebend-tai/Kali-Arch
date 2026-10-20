namespace CleanArch.Application.Interfaces.Strategies;

/// <summary>
/// Strategy — different algorithms for picking a provider at runtime.
/// Cost-optimized, latency-optimized, region-pinned, etc.
/// </summary>
public interface IProviderSelectionStrategy
{
    Task<string> SelectProviderAsync(
        ProviderSelectionContext context,
        CancellationToken ct = default);
}

public sealed record ProviderSelectionContext(
    string ResourceType,
    string Workload,
    string? RegionHint = null);
