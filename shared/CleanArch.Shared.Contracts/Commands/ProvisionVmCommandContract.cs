namespace CleanArch.Shared.Contracts.Commands;

/// <summary>
/// Queue message contract — consumed by the vm-provision queue trigger.
/// Named ...Contract to avoid colliding with Application's
/// MediatR ProvisionVmCommand.
/// </summary>
public sealed record ProvisionVmCommandContract(
    string Name,
    string ResourceGroup,
    string Region,
    string OsType,
    string Size);
