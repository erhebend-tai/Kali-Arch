namespace CleanArch.Shared.Abstractions.DTOs.Compute;

/// <summary>Workload deployment onto a container orchestrator.</summary>
public sealed record ContainerDeploymentRequest(
    string ClusterId,
    string ServiceName,
    string Image,
    int Replicas = 1,
    IReadOnlyDictionary<string, string>? EnvironmentVariables = null);
