namespace CleanArch.Shared.Contracts.Events.Compute;

public sealed record VmStartedIntegrationEvent(
    Guid VmId,
    string VmName,
    DateTime OccurredAt);
