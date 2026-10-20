using CleanArch.Application.Interfaces;
using CleanArch.Domain.Factories;
using CleanArch.Domain.Interfaces.Repositories.Compute;

namespace CleanArch.Application.Pipelines.Compute.Steps;

public sealed class ProvisionVmStep(
    IVirtualMachineFactory vmFactory,
    IVirtualMachineRepository vmRepository,
    IVirtualMachineService vmService)
    : IPipelineStep<VmProvisioningContext>
{
    public int Order => 4;
    public string Name => "ProvisionVm";

    public async Task<StepResult> ExecuteAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        // Factory creates the aggregate — domain events raised.
        var vm = vmFactory.Create(
            context.Request.Name,
            context.Request.Region,
            context.Request.Size,
            context.Request.OsType,
            context.Request.ResourceGroup,
            context.UserId);

        // Provider does the actual provisioning.
        await vmService.ProvisionAsync(
            context.Request.Name,
            context.Request.Region,
            context.Request.Size,
            context.Request.OsType,
            context.Request.ResourceGroup,
            ct);

        await vmRepository.AddAsync(vm, ct);
        context.CreatedVm = vm;
        return StepResult.Success();
    }

    public async Task CompensateAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        if (context.CreatedVm is not null)
            await vmService.DeleteAsync(context.CreatedVm.Name.Value, ct);
    }
}
