using CleanArch.Shared.Domain.Primitives;

namespace VirtualMachineService.Domain.Entities.Compute;

/// <summary>Point-in-time VM snapshot — mirrors vm_service.vm_snapshots.</summary>
public sealed class VmSnapshot : AuditableEntity
{
    public Guid VmId { get; private set; }
    public string TenantId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int SizeGb { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime? ExpiresAt { get; private set; }

    private VmSnapshot() { }

    public static VmSnapshot Create(
        Guid vmId,
        string tenantId,
        string name,
        int sizeGb,
        string createdBy,
        DateTime? expiresAt = null) =>
        new()
        {
            VmId      = vmId,
            TenantId  = tenantId,
            Name      = name,
            SizeGb    = sizeGb,
            CreatedBy = createdBy,
            ExpiresAt = expiresAt
        };
}
