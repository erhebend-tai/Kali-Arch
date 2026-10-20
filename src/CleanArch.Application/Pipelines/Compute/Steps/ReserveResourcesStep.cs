namespace CleanArch.Application.Pipelines.Compute.Steps;

public sealed class ReserveResourcesStep
    : IPipelineStep<VmProvisioningContext>
{
    public int Order => 3;
    public string Name => "ReserveResources";

    public Task<StepResult> ExecuteAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        // Provider-level reservation call goes here.
        context.ReservationId = Guid.NewGuid().ToString();
        return Task.FromResult(StepResult.Success());
    }

    public Task CompensateAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        // Release reservation — best effort.
        context.ReservationId = null;
        return Task.CompletedTask;
    }
}
