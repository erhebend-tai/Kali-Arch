using CleanArch.Shared.Abstractions.DTOs.Strategies;

namespace CleanArch.Shared.Abstractions.Application.Strategies;

/// <summary>
/// Cost comparison across providers - each strategy gathers
/// prices for a workload in a region.
/// </summary>
public interface IPricingStrategy
{
    string Name { get; }

    Task<IReadOnlyList<ProviderPrice>> GetPricesAsync(
        string region, string size,
        CancellationToken ct = default);
}
