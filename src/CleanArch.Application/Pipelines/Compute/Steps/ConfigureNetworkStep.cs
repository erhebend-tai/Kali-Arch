namespace CleanArch.Application.Pipelines.Compute.Steps;

public sealed class ConfigureNetworkStep
    : IPipelineStep<VmProvisioningContext>
{
    public int Order => 5;
    public string Name => "ConfigureNetwork";

    public Task<StepResult> ExecuteAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        // Provider network wiring — NIC, NSG/firewall rules.
        context.NetworkInterfaceId = Guid.NewGuid().ToString();
        return Task.FromResult(StepResult.Success());
    }

    public Task CompensateAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        context.NetworkInterfaceId = null;
        return Task.CompletedTask;
    }
}
