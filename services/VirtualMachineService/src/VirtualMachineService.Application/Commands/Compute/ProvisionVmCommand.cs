using CleanArch.Shared.Results;
using VirtualMachineService.Application.Common;

namespace VirtualMachineService.Application.Commands.Compute;

/// <summary>
/// MediatR command — write side of CQRS.
/// Controller/gRPC/function entry points all ISender.Send() this.
/// </summary>
public sealed record ProvisionVmCommand(
    string Name,
    string Region,
    string Size,
    string OsType,
    string ResourceGroup,
    string RequestedBy,
    string TenantId) : ICommand<Result<ProvisionVmResult>>;

public sealed record ProvisionVmResult(
    Guid VmId,
    string Name,
    string Status);
