using CleanArch.Shared.Abstractions.DTOs.Compute;

namespace CleanArch.Shared.Abstractions.Application.Compute;

/// <summary>
/// Provider-agnostic VM lifecycle - implemented per cloud provider.
/// </summary>
public interface IVirtualMachineService
{
    Task<string> ProvisionAsync(
        VmProvisionRequest request, CancellationToken ct = default);

    Task StartAsync(string vmId, CancellationToken ct = default);
    Task StopAsync(string vmId, CancellationToken ct = default);
    Task DeallocateAsync(string vmId, CancellationToken ct = default);
    Task DeleteAsync(string vmId, CancellationToken ct = default);

    Task ResizeAsync(
        string vmId, string newSize, CancellationToken ct = default);

    Task<bool> IsHealthyAsync(
        string vmId, CancellationToken ct = default);

    Task<VmMetrics> GetMetricsAsync(
        string vmId, CancellationToken ct = default);

    Task<string> GetStatusAsync(
        string vmId, CancellationToken ct = default);
}
