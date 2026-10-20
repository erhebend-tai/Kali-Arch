using CleanArch.Shared.Abstractions.DTOs.Compute;

namespace CleanArch.Shared.Abstractions.Application.Compute;

/// <summary>
/// Container orchestration - Kubernetes / AKS / EKS / GKE / ACK / OKE.
/// </summary>
public interface IContainerOrchestrationService
{
    Task<string> DeployAsync(
        ContainerDeploymentRequest request,
        CancellationToken ct = default);

    Task ScaleAsync(
        string deploymentId, int replicas,
        CancellationToken ct = default);

    Task<DeploymentStatus> GetStatusAsync(
        string deploymentId, CancellationToken ct = default);

    Task DeleteAsync(
        string deploymentId, CancellationToken ct = default);
}
