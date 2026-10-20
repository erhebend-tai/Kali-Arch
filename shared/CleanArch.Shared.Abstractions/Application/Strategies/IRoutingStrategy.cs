namespace CleanArch.Shared.Abstractions.Application.Strategies;

/// <summary>
/// Service discovery / endpoint resolution - DNS, Dapr,
/// gateway routing table, or static config.
/// </summary>
public interface IRoutingStrategy
{
    string Name { get; }

    Task<string> ResolveEndpointAsync(
        string serviceName, CancellationToken ct = default);
}
