using CleanArch.Application.Common;

namespace CleanArch.Application.Commands.Compute;

/// <summary>
/// MediatR command — write side of CQRS.
/// Controller/function/worker entry points all ISender.Send() this.
/// </summary>
public sealed record ProvisionVmCommand(
    string Name,
    string ResourceGroup,
    string Region,
    string OsType,
    string Size) : ICommand<ProvisionVmResult>;

public sealed record ProvisionVmResult(
    Guid VmId,
    string Name,
    string Status,
    DateTime CreatedAt);
