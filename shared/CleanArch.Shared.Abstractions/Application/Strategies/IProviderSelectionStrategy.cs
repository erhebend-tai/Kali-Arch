using CleanArch.Shared.Abstractions.DTOs.Strategies;

namespace CleanArch.Shared.Abstractions.Application.Strategies;

/// <summary>
/// Strategy - different algorithms for picking a provider at runtime.
/// Cost-optimized, latency-optimized, region-pinned, etc.
/// </summary>
public interface IProviderSelectionStrategy
{
    string Name { get; }

    Task<string> SelectProviderAsync(
        ProviderSelectionContext context,
        CancellationToken ct = default);
}
