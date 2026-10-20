namespace CleanArch.Application.Pipelines.Compute.Steps;

public sealed class InstallAgentStep
    : IPipelineStep<VmProvisioningContext>
{
    public int Order => 6;
    public string Name => "InstallAgent";

    public Task<StepResult> ExecuteAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        context.AgentInstallId = Guid.NewGuid().ToString();
        return Task.FromResult(StepResult.Success());
    }

    public Task CompensateAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        context.AgentInstallId = null;
        return Task.CompletedTask;
    }
}
