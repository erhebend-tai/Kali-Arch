namespace CleanArch.Shared.Abstractions.DTOs.Compute;

public enum DeploymentStatus
{
    Pending,
    Deploying,
    Running,
    Degraded,
    Failed,
    Deleted
}
