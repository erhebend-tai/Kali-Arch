using CleanArch.Application.Common;
using CleanArch.Domain.Interfaces.Repositories.Compute;
using CleanArch.Domain.Specifications.Compute;

namespace CleanArch.Application.Queries.Compute;

public sealed record GetAllVmsQuery(
    string? Region = null,
    bool RunningOnly = false) : IQuery<IReadOnlyList<VmDetails>>;

public sealed class GetAllVmsQueryHandler(IVirtualMachineRepository vmRepository)
    : IQueryHandler<GetAllVmsQuery, IReadOnlyList<VmDetails>>
{
    public async Task<IReadOnlyList<VmDetails>> Handle(
        GetAllVmsQuery request, CancellationToken ct)
    {
        // Specifications compose — domain decides the filter rules.
        Domain.Specifications.ISpecification<Domain.Entities.Compute.VirtualMachine>? spec = null;

        if (request.RunningOnly)
            spec = new RunningVmsSpecification();

        if (request.Region is not null)
        {
            var regionSpec =
                new VmsByRegionSpecification(new Domain.ValueObjects.Compute.Region(request.Region));
            spec = spec is null ? regionSpec : spec.And(regionSpec);
        }

        var vms = spec is null
            ? await vmRepository.GetAllAsync(ct)
            : await vmRepository.FindAsync(spec, ct);

        return vms.Select(vm => new VmDetails(
            vm.Id,
            vm.Name.Value,
            vm.Region.Value,
            vm.Size.Value,
            vm.OsType.ToString(),
            vm.Status.ToString(),
            vm.ResourceGroup,
            vm.CreatedAt)).ToList();
    }
}
