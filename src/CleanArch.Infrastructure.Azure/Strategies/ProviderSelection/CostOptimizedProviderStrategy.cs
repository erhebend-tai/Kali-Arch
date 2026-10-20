using CleanArch.Application.Interfaces;
using CleanArch.Application.Interfaces.Strategies;

namespace CleanArch.Infrastructure.Azure.Strategies.ProviderSelection;

/// <summary>
/// Strategy — picks the cheapest provider for a given workload.
/// </summary>
public sealed class CostOptimizedProviderStrategy(
    IPricingService pricingService)
    : IProviderSelectionStrategy
{
    public async Task<string> SelectProviderAsync(
        ProviderSelectionContext context, CancellationToken ct = default)
    {
        var candidates = new[] { "azure", "aws", "gcp" };
        var best = "azure";
        var bestCost = decimal.MaxValue;

        foreach (var candidate in candidates)
        {
            var cost = await pricingService.GetMonthlyEstimateAsync(
                context.ResourceType,
                "standard",
                context.RegionHint ?? "us-east",
                ct);

            if (cost < bestCost)
            {
                bestCost = cost;
                best = candidate;
            }
        }

        return best;
    }
}
