using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Enums.Compute;
using CleanArch.Domain.Interfaces.Repositories.Compute;
using CleanArch.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Common.Persistence;

public class VirtualMachineRepository(AppDbContext context)
    : IVirtualMachineRepository
{
    public async Task<VirtualMachine?> GetByIdAsync(
        Guid id, CancellationToken ct = default) =>
        await context.VirtualMachines
            .FirstOrDefaultAsync(vm => vm.Id == id, ct);

    public async Task<IReadOnlyList<VirtualMachine>> GetAllAsync(
        CancellationToken ct = default) =>
        await context.VirtualMachines.ToListAsync(ct);

    public async Task<IReadOnlyList<VirtualMachine>> FindAsync(
        ISpecification<VirtualMachine> specification,
        CancellationToken ct = default) =>
        await SpecificationEvaluator<VirtualMachine>
            .GetQuery(context.VirtualMachines, specification)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<VirtualMachine>> GetByStatusAsync(
        VmStatus status, CancellationToken ct = default) =>
        await context.VirtualMachines
            .Where(vm => vm.Status == status)
            .ToListAsync(ct);

    public async Task AddAsync(
        VirtualMachine entity, CancellationToken ct = default) =>
        await context.VirtualMachines.AddAsync(entity, ct);

    public Task UpdateAsync(
        VirtualMachine entity, CancellationToken ct = default)
    {
        context.VirtualMachines.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is not null)
            context.VirtualMachines.Remove(entity);
    }
}
