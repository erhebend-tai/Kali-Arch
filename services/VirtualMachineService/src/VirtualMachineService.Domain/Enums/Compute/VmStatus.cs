namespace VirtualMachineService.Domain.Enums.Compute;

/// <summary>Mirrors the chk_vm_status constraint in V002__create_tables.sql.</summary>
public enum VmStatus
{
    Stopped,
    Running,
    Deallocated,
    Provisioning,
    Failed,
    Unknown
}
