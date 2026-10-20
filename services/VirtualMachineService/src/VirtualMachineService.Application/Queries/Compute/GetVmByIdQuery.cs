using CleanArch.Shared.Results;
using VirtualMachineService.Application.Common;
using VirtualMachineService.Application.DTOs.Compute;
using VirtualMachineService.Application.Interfaces;

namespace VirtualMachineService.Application.Queries.Compute;

public sealed record GetVmByIdQuery(
    Guid VmId,
    string TenantId) : IQuery<Result<VmDetailsDto>>;

public sealed class GetVmByIdQueryHandler(
    IVirtualMachineRepository repository)
    : IQueryHandler<GetVmByIdQuery, Result<VmDetailsDto>>
{
    public async Task<Result<VmDetailsDto>> Handle(
        GetVmByIdQuery request, CancellationToken ct)
    {
        var vm = await repository.GetByIdAsync(request.VmId, ct);

        if (vm is null || vm.IsDeleted || vm.TenantId != request.TenantId)
            return Error.NotFound(
                "Vm.NotFound", $"VM '{request.VmId}' was not found.");

        return new VmDetailsDto(
            vm.Id,
            vm.TenantId,
            vm.Name.Value,
            vm.Region.Value,
            vm.Size.Value,
            vm.OsType.ToString(),
            vm.Status.ToString().ToLowerInvariant(),
            vm.ResourceGroup,
            vm.Provider,
            vm.CreatedBy,
            vm.CreatedAt,
            vm.UpdatedAt);
    }
}
