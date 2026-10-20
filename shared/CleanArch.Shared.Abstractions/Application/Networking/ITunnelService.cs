using CleanArch.Shared.Abstractions.DTOs.Networking;

namespace CleanArch.Shared.Abstractions.Application.Networking;

/// <summary>
/// Ingress tunnels - Cloudflared / WireGuard / Tailscale.
/// </summary>
public interface ITunnelService
{
    Task<TunnelInfo> CreateTunnelAsync(
        TunnelConfig config, CancellationToken ct = default);

    Task<TunnelStatus> GetStatusAsync(
        string tunnelId, CancellationToken ct = default);

    Task DeleteTunnelAsync(
        string tunnelId, CancellationToken ct = default);

    Task<IReadOnlyList<TunnelInfo>> ListTunnelsAsync(
        CancellationToken ct = default);
}
