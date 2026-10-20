using CleanArch.Domain.Interfaces.Repositories.Compute;
using CleanArch.Domain.ValueObjects.Compute;

namespace CleanArch.Application.Pipelines.Compute.Steps;

public sealed class ValidateVmNameStep(IVirtualMachineRepository vmRepository)
    : IPipelineStep<VmProvisioningContext>
{
    public int Order => 1;
    public string Name => "ValidateVmName";

    public async Task<StepResult> ExecuteAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        // Value object construction validates the name format.
        _ = new ResourceName(context.Request.Name);

        var existing = await vmRepository.GetAllAsync(ct);
        if (existing.Any(vm => vm.Name.Value == context.Request.Name))
            return StepResult.Failure(
                $"VM name '{context.Request.Name}' is already taken.");

        return StepResult.Success();
    }

    public Task CompensateAsync(
        VmProvisioningContext context, CancellationToken ct) =>
        Task.CompletedTask; // validation produces no side effects
}
