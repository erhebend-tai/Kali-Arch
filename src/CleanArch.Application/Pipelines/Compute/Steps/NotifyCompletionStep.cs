using CleanArch.Application.Interfaces;

namespace CleanArch.Application.Pipelines.Compute.Steps;

public sealed class NotifyCompletionStep(IMessageBusService messageBusService)
    : IPipelineStep<VmProvisioningContext>
{
    public int Order => 7;
    public string Name => "NotifyCompletion";

    public async Task<StepResult> ExecuteAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        await messageBusService.PublishAsync(
            "vm.provisioning.complete",
            new { VmId = context.CreatedVm?.Id },
            ct);
        return StepResult.Success();
    }

    public Task CompensateAsync(
        VmProvisioningContext context, CancellationToken ct) =>
        Task.CompletedTask;
}
