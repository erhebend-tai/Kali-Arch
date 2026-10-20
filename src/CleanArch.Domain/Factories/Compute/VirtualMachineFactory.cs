using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Enums.Compute;
using CleanArch.Domain.Exceptions;
using CleanArch.Domain.ValueObjects.Compute;

namespace CleanArch.Domain.Factories.Compute;

public sealed class VirtualMachineFactory : IVirtualMachineFactory
{
    public VirtualMachine Create(
        string name,
        string region,
        string size,
        string osType,
        string resourceGroup,
        string createdBy)
    {
        // All primitive→value-object conversion happens here,
        // not in the entity constructor.
        return VirtualMachine.Create(
            new ResourceName(name),
            new Region(region),
            new MachineSize(size),
            ParseOsType(osType),
            resourceGroup,
            createdBy);
    }

    public VirtualMachine Reconstitute(
        Guid id, string name, string region, string size,
        string osType, VmStatus status, string resourceGroup,
        DateTime createdAt) =>
        VirtualMachine.Reconstitute(
            id,
            new ResourceName(name),
            new Region(region),
            new MachineSize(size),
            ParseOsType(osType),
            status,
            resourceGroup,
            createdAt);

    private static OsType ParseOsType(string osType) =>
        osType.ToLowerInvariant() switch
        {
            "linux"   => OsType.Linux,
            "windows" => OsType.Windows,
            _ => throw new InvalidDomainStateException(
                $"Unknown OS type: '{osType}'")
        };
}
