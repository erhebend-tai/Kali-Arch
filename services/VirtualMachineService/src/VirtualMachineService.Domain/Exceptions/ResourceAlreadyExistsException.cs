namespace VirtualMachineService.Domain.Exceptions;

/// <summary>
/// Raised when a resource with the same identity already exists —
/// mirrors the DUPLICATE_VM error from sp_provision_vm.
/// </summary>
public sealed class ResourceAlreadyExistsException(string resourceName)
    : DomainException($"Resource '{resourceName}' already exists.")
{
    public string ResourceName { get; } = resourceName;
}
