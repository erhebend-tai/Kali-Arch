using CleanArch.Shared.Contracts.Versioning;

namespace CleanArch.Shared.Contracts.Events.Compute;

/// <summary>
/// Integration event — crosses service boundaries via the message bus.
/// Primitives only, never domain objects. V1 contract.
/// </summary>
[EventVersion("vm-provisioned", 1)]
public sealed record VmProvisionedIntegrationEvent(
    Guid VmId,
    string VmName,
    string Region,
    string ResourceGroup,
    DateTime OccurredAt);
