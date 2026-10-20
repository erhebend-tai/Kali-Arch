namespace CleanArch.Shared.Abstractions.Application.Networking;

/// <summary>
/// Load balancer management - ALB / ELB / Cloudflare LB /
/// bare-metal nginx or HAProxy.
/// </summary>
public interface ILoadBalancerService
{
    Task<string> CreateOrUpdateAsync(
        string name, IReadOnlyList<string> backends,
        CancellationToken ct = default);

    Task AddBackendAsync(
        string loadBalancerId, string backendAddress,
        CancellationToken ct = default);

    Task RemoveBackendAsync(
        string loadBalancerId, string backendAddress,
        CancellationToken ct = default);

    Task<string> GetStatusAsync(
        string loadBalancerId, CancellationToken ct = default);
}
