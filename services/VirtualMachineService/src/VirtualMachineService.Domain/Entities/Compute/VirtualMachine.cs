using CleanArch.Shared.Domain.Primitives;
using VirtualMachineService.Domain.Enums.Compute;
using VirtualMachineService.Domain.Events.Compute;
using VirtualMachineService.Domain.Exceptions;
using VirtualMachineService.Domain.ValueObjects.Compute;

namespace VirtualMachineService.Domain.Entities.Compute;

/// <summary>
/// VM aggregate root — all state changes go through methods,
/// never setters. Mirrors vm_service.virtual_machines.
/// </summary>
public sealed class VirtualMachine : AuditableEntity
{
    public string TenantId { get; private set; } = string.Empty;
    public ResourceName Name { get; private set; } = null!;
    public Region Region { get; private set; } = null!;
    public MachineSize Size { get; private set; } = null!;
    public OsType OsType { get; private set; }
    public VmStatus Status { get; private set; }
    public string ResourceGroup { get; private set; } = string.Empty;
    public string Provider { get; private set; } = "Azure";
    public string CreatedBy { get; private set; } = string.Empty;
    public string? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }
    public long RowVersion { get; private set; }

    private VirtualMachine() { }

    public static VirtualMachine Create(
        string tenantId,
        ResourceName name,
        Region region,
        MachineSize size,
        OsType osType,
        string resourceGroup,
        string provider,
        string createdBy)
    {
        var vm = new VirtualMachine
        {
            TenantId      = tenantId,
            Name          = name,
            Region        = region,
            Size          = size,
            OsType        = osType,
            Status        = VmStatus.Provisioning,
            ResourceGroup = resourceGroup,
            Provider      = provider,
            CreatedBy     = createdBy
        };

        vm.RaiseDomainEvent(new VmProvisionedEvent(vm.Id, name.Value));
        return vm;
    }

    public void Start()
    {
        if (Status == VmStatus.Running)
            throw new InvalidDomainStateException("VM is already running.");

        Status = VmStatus.Running;
        Touch();
        RaiseDomainEvent(new VmStartedEvent(Id));
    }

    public void Stop()
    {
        if (Status == VmStatus.Stopped)
            throw new InvalidDomainStateException("VM is already stopped.");

        Status = VmStatus.Stopped;
        Touch();
    }

    public void Deallocate()
    {
        if (Status != VmStatus.Stopped)
            throw new InvalidDomainStateException(
                "VM must be stopped before deallocating.");

        Status = VmStatus.Deallocated;
        Touch();
    }

    public void MarkFailed()
    {
        Status = VmStatus.Failed;
        Touch();
    }

    public void Resize(MachineSize newSize)
    {
        if (Status == VmStatus.Running)
            throw new InvalidDomainStateException(
                "VM must be stopped before resizing.");

        Size = newSize;
        Touch();
    }

    /// <summary>
    /// Soft delete — the trg_vm_soft_delete trigger converts
    /// physical DELETEs into this state at the database level.
    /// </summary>
    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        Touch();
    }
}
