using Microsoft.AspNetCore.Http;

namespace CleanArch.Shared.Abstractions.Application.Strategies;

/// <summary>
/// Extracts the tenant from an incoming request -
/// subdomain, header, claim, or route value.
/// </summary>
public interface ITenantResolutionStrategy
{
    Task<string?> ResolveAsync(
        HttpContext context, CancellationToken ct = default);
}
