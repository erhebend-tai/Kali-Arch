using CleanArch.Application.Pipelines.Compute.Steps;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Pipelines.Compute;

/// <summary>
/// Static step list resolved from DI — swap steps by swapping
/// the registered step implementations.
/// </summary>
public sealed class VmProvisioningPipeline(
    ValidateVmNameStep validateVmName,
    CheckQuotaStep checkQuota,
    ReserveResourcesStep reserveResources,
    ProvisionVmStep provisionVm,
    ConfigureNetworkStep configureNetwork,
    InstallAgentStep installAgent,
    NotifyCompletionStep notifyCompletion,
    ILogger<VmProvisioningPipeline> logger)
    : Pipeline<VmProvisioningContext>(logger)
{
    protected override IReadOnlyList<IPipelineStep<VmProvisioningContext>> Steps { get; } =
    [
        validateVmName,
        checkQuota,
        reserveResources,
        provisionVm,
        configureNetwork,
        installAgent,
        notifyCompletion
    ];
}
