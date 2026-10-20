using CleanArch.Shared.Results;
using VirtualMachineService.Application.Common;

namespace VirtualMachineService.Application.Commands.Compute;

public sealed record VmActionResult(
    Guid VmId,
    string Status,
    string Message);

public sealed record StartVmCommand(
    Guid VmId,
    string TenantId) : ICommand<Result<VmActionResult>>;

public sealed record StopVmCommand(
    Guid VmId,
    string TenantId) : ICommand<Result<VmActionResult>>;

public sealed record DeallocateVmCommand(
    Guid VmId,
    string TenantId) : ICommand<Result<VmActionResult>>;

public sealed record ResizeVmCommand(
    Guid VmId,
    string NewSize,
    string TenantId) : ICommand<Result<VmActionResult>>;

public sealed record DeleteVmCommand(
    Guid VmId,
    string TenantId,
    string DeletedBy) : ICommand<Result>;
