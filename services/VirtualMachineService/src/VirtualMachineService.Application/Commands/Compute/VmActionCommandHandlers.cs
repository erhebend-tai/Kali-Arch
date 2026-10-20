using CleanArch.Shared.Results;
using VirtualMachineService.Application.Common;
using VirtualMachineService.Application.Interfaces;
using VirtualMachineService.Domain.Entities.Compute;
using VirtualMachineService.Domain.Exceptions;
using VirtualMachineService.Domain.ValueObjects.Compute;

namespace VirtualMachineService.Application.Commands.Compute;

/// <summary>
/// Shared load-and-mutate flow for lifecycle commands —
/// loads the aggregate, applies the domain method, persists.
/// </summary>
internal static class VmActionHelpers
{
    public static async Task<Result<VmActionResult>> ApplyAsync(
        IVirtualMachineRepository repository,
        Guid vmId,
        string tenantId,
        Action<VirtualMachine> action,
        CancellationToken ct)
    {
        var vm = await repository.GetByIdAsync(vmId, ct);

        if (vm is null || vm.IsDeleted)
            return Error.NotFound(
                "Vm.NotFound", $"VM '{vmId}' was not found.");

        if (vm.TenantId != tenantId)
            return Error.Forbidden(
                "Vm.WrongTenant",
                $"VM '{vmId}' does not belong to tenant '{tenantId}'.");

        try
        {
            action(vm);
        }
        catch (DomainException ex)
        {
            return Error.Domain("Vm.InvalidState", ex.Message);
        }

        await repository.UpdateAsync(vm, ct);

        return new VmActionResult(
            vm.Id,
            vm.Status.ToString().ToLowerInvariant(),
            "OK");
    }
}

public sealed class StartVmCommandHandler(
    IVirtualMachineRepository repository)
    : ICommandHandler<StartVmCommand, Result<VmActionResult>>
{
    public Task<Result<VmActionResult>> Handle(
        StartVmCommand request, CancellationToken ct) =>
        VmActionHelpers.ApplyAsync(
            repository, request.VmId, request.TenantId,
            vm => vm.Start(), ct);
}

public sealed class StopVmCommandHandler(
    IVirtualMachineRepository repository)
    : ICommandHandler<StopVmCommand, Result<VmActionResult>>
{
    public Task<Result<VmActionResult>> Handle(
        StopVmCommand request, CancellationToken ct) =>
        VmActionHelpers.ApplyAsync(
            repository, request.VmId, request.TenantId,
            vm => vm.Stop(), ct);
}

public sealed class DeallocateVmCommandHandler(
    IVirtualMachineRepository repository)
    : ICommandHandler<DeallocateVmCommand, Result<VmActionResult>>
{
    public Task<Result<VmActionResult>> Handle(
        DeallocateVmCommand request, CancellationToken ct) =>
        VmActionHelpers.ApplyAsync(
            repository, request.VmId, request.TenantId,
            vm => vm.Deallocate(), ct);
}

public sealed class ResizeVmCommandHandler(
    IVirtualMachineRepository repository)
    : ICommandHandler<ResizeVmCommand, Result<VmActionResult>>
{
    public Task<Result<VmActionResult>> Handle(
        ResizeVmCommand request, CancellationToken ct) =>
        VmActionHelpers.ApplyAsync(
            repository, request.VmId, request.TenantId,
            vm => vm.Resize(new MachineSize(request.NewSize)), ct);
}

public sealed class DeleteVmCommandHandler(
    IVirtualMachineRepository repository)
    : ICommandHandler<DeleteVmCommand, Result>
{
    public async Task<Result> Handle(
        DeleteVmCommand request, CancellationToken ct)
    {
        var vm = await repository.GetByIdAsync(request.VmId, ct);

        if (vm is null || vm.IsDeleted)
            return Error.NotFound(
                "Vm.NotFound", $"VM '{request.VmId}' was not found.");

        if (vm.TenantId != request.TenantId)
            return Error.Forbidden(
                "Vm.WrongTenant",
                $"VM '{request.VmId}' does not belong to " +
                $"tenant '{request.TenantId}'.");

        vm.SoftDelete(request.DeletedBy);
        await repository.UpdateAsync(vm, ct);

        return Result.Success();
    }
}
