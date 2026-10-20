using CleanArch.Domain.Enums.Compute;
using CleanArch.Domain.Events.Compute;
using CleanArch.Domain.Exceptions;
using CleanArch.Domain.ValueObjects.Compute;
using CleanArch.Shared.Domain.Primitives;

namespace CleanArch.Domain.Entities.Compute;

/// <summary>
/// VM aggregate root — all state changes go through methods,
/// never setters. Domain events raised on every meaningful transition.
/// </summary>
public sealed class VirtualMachine : AuditableEntity
{
    public ResourceName Name { get; private set; } = null!;
    public Region Region { get; private set; } = null!;
    public MachineSize Size { get; private set; } = null!;
    public OsType OsType { get; private set; }
    public VmStatus Status { get; private set; }
    public string ResourceGroup { get; private set; } = string.Empty;
    public string CreatedBy { get; private set; } = string.Empty;

    private VirtualMachine() { }

    public static VirtualMachine Create(
        ResourceName name,
        Region region,
        MachineSize size,
        OsType osType,
        string resourceGroup,
        string createdBy)
    {
        var vm = new VirtualMachine
        {
            Name          = name,
            Region        = region,
            Size          = size,
            OsType        = osType,
            Status        = VmStatus.Stopped,
            ResourceGroup = resourceGroup,
            CreatedBy     = createdBy
        };

        vm.RaiseDomainEvent(new VmProvisionedEvent(vm.Id, name.Value));
        return vm;
    }

    /// <summary>Rehydrate from persistence — no domain events.</summary>
    public static VirtualMachine Reconstitute(
        Guid id,
        ResourceName name,
        Region region,
        MachineSize size,
        OsType osType,
        VmStatus status,
        string resourceGroup,
        DateTime createdAt) =>
        new()
        {
            Id            = id,
            Name          = name,
            Region        = region,
            Size          = size,
            OsType        = osType,
            Status        = status,
            ResourceGroup = resourceGroup,
            CreatedAt     = createdAt
        };

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

    public void Resize(MachineSize newSize)
    {
        if (Status == VmStatus.Running)
            throw new InvalidDomainStateException(
                "VM must be stopped before resizing.");

        Size = newSize;
        Touch();
    }
}
