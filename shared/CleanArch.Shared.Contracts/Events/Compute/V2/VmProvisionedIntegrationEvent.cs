using CleanArch.Shared.Contracts.Versioning;

namespace CleanArch.Shared.Contracts.Events.Compute.V2;

/// <summary>
/// V2 — adds Size, OsType, ProvisionedBy. Backward compatible:
/// V1 consumers ignore the new fields.
/// </summary>
[EventVersion("vm-provisioned", 2)]
public sealed record VmProvisionedIntegrationEvent(
    Guid VmId,
    string VmName,
    string Region,
    string ResourceGroup,
    string Size,
    string OsType,
    string ProvisionedBy,
    DateTime OccurredAt);
