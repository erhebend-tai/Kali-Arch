namespace VirtualMachineService.Domain.Exceptions;

/// <summary>
/// Raised when a tenant exceeds its VM quota — mirrors the
/// QUOTA_EXCEEDED error from sp_provision_vm.
/// </summary>
public sealed class QuotaExceededException(string tenantId)
    : DomainException($"Tenant '{tenantId}' has reached its VM quota.")
{
    public string TenantId { get; } = tenantId;
}
