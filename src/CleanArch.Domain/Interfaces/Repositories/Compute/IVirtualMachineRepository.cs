using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Enums.Compute;
using CleanArch.Domain.Specifications;

namespace CleanArch.Domain.Interfaces.Repositories.Compute;

public interface IVirtualMachineRepository : IRepository<VirtualMachine>
{
    Task<IReadOnlyList<VirtualMachine>> FindAsync(
        ISpecification<VirtualMachine> specification,
        CancellationToken ct = default);

    Task<IReadOnlyList<VirtualMachine>> GetByStatusAsync(
        VmStatus status, CancellationToken ct = default);
}
