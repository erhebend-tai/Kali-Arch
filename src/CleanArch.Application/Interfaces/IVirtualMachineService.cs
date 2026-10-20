namespace CleanArch.Application.Interfaces;

/// <summary>
/// Provider-agnostic VM operations — implemented per cloud provider.
/// </summary>
public interface IVirtualMachineService
{
    Task<string> ProvisionAsync(
        string name,
        string region,
        string size,
        string osType,
        string resourceGroup,
        CancellationToken ct = default);

    Task StartAsync(string name, CancellationToken ct = default);
    Task StopAsync(string name, CancellationToken ct = default);
    Task DeleteAsync(string name, CancellationToken ct = default);
    Task<string> GetStatusAsync(string name, CancellationToken ct = default);
}
