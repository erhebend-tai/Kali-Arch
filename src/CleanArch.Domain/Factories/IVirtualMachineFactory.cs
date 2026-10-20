using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Enums.Compute;

namespace CleanArch.Domain.Factories;

/// <summary>
/// Pure domain factory — validates inputs and creates entities.
/// No cloud SDK knowledge.
/// </summary>
public interface IVirtualMachineFactory
{
    VirtualMachine Create(
        string name,
        string region,
        string size,
        string osType,
        string resourceGroup,
        string createdBy);

    /// <summary>Rehydrate an existing entity — no domain events raised.</summary>
    VirtualMachine Reconstitute(
        Guid id,
        string name,
        string region,
        string size,
        string osType,
        VmStatus status,
        string resourceGroup,
        DateTime createdAt);
}
