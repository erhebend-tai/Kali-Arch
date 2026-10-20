using CleanArch.Application.Common;
using CleanArch.Domain.Interfaces.Repositories.Compute;

namespace CleanArch.Application.Queries.Compute;

public sealed class GetVmByIdQueryHandler(IVirtualMachineRepository vmRepository)
    : IQueryHandler<GetVmByIdQuery, VmDetails?>
{
    public async Task<VmDetails?> Handle(
        GetVmByIdQuery request, CancellationToken ct)
    {
        var vm = await vmRepository.GetByIdAsync(request.VmId, ct);
        if (vm is null) return null;

        return new VmDetails(
            vm.Id,
            vm.Name.Value,
            vm.Region.Value,
            vm.Size.Value,
            vm.OsType.ToString(),
            vm.Status.ToString(),
            vm.ResourceGroup,
            vm.CreatedAt);
    }
}
