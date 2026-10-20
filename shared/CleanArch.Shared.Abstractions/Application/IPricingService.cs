namespace CleanArch.Shared.Abstractions.Application;

/// <summary>
/// Provider pricing lookup - each provider exposes its own catalog API.
/// </summary>
public interface IPricingService
{
    /// <summary>Monthly estimate in USD for the given workload.</summary>
    Task<decimal> GetMonthlyEstimateAsync(
        string resourceType,
        string sku,
        string region,
        CancellationToken ct = default);
}
