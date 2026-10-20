using CleanArch.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Services;

/// <summary>
/// Dev/test VM service fallback — simulates a provider round-trip
/// and logs the operation. Cloud providers replace this with real
/// SDK implementations (Azure Compute, EC2, GCE, ECS, OCI, CF).
/// </summary>
public sealed class LoggingVirtualMachineService(
    ILogger<LoggingVirtualMachineService> logger)
    : IVirtualMachineService
{
    public Task<string> ProvisionAsync(
        string name, string region, string size, string osType,
        string resourceGroup, CancellationToken ct = default)
    {
        var providerId = $"sim-{Guid.NewGuid():N}"[..16];
        logger.LogInformation(
            "[VM] Provision {Name} ({Size}, {Os}) in {Region}/{RG} → {Id}",
            name, size, osType, region, resourceGroup, providerId);
        return Task.FromResult(providerId);
    }

    public Task StartAsync(string name, CancellationToken ct = default)
    {
        logger.LogInformation("[VM] Start {Name}", name);
        return Task.CompletedTask;
    }

    public Task StopAsync(string name, CancellationToken ct = default)
    {
        logger.LogInformation("[VM] Stop {Name}", name);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string name, CancellationToken ct = default)
    {
        logger.LogInformation("[VM] Delete {Name}", name);
        return Task.CompletedTask;
    }

    public Task<string> GetStatusAsync(
        string name, CancellationToken ct = default)
    {
        logger.LogInformation("[VM] Status {Name}", name);
        return Task.FromResult("Running");
    }
}
