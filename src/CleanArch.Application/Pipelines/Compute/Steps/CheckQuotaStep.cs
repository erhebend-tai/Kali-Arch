using CleanArch.Domain.Interfaces;

namespace CleanArch.Application.Pipelines.Compute.Steps;

public sealed class CheckQuotaStep(IQuotaService quotaService)
    : IPipelineStep<VmProvisioningContext>
{
    public int Order => 2;
    public string Name => "CheckQuota";

    public async Task<StepResult> ExecuteAsync(
        VmProvisioningContext context, CancellationToken ct)
    {
        var hasQuota = await quotaService.HasAvailableQuotaAsync(
            context.Request.Region, context.Request.Size, ct);

        return hasQuota
            ? StepResult.Success()
            : StepResult.Failure(
                $"Quota exceeded in '{context.Request.Region}'.");
    }

    public Task CompensateAsync(
        VmProvisioningContext context, CancellationToken ct) =>
        Task.CompletedTask;
}
