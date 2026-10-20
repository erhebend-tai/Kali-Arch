using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Exceptions;
using CleanArch.Domain.Factories;
using CleanArch.Domain.Interfaces;

namespace CleanArch.Domain.Templates;

public sealed class VmProvisioningTemplate(
    IVirtualMachineFactory factory,
    IQuotaService quotaService)
    : ResourceProvisioningTemplate<VirtualMachine>
{
    protected override async Task CheckAvailabilityAsync(
        string region, CancellationToken ct)
    {
        var available = await quotaService.HasAvailableQuotaAsync(
            region, "medium", ct);

        if (!available)
            throw new QuotaExceededException(region);
    }

    protected override Task<VirtualMachine> CreateResourceAsync(
        string name, string region, CancellationToken ct) =>
        Task.FromResult(factory.Create(
            name, region, "medium", "Linux", "default-rg", "system"));
}
