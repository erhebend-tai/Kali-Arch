namespace CleanArch.Shared.Abstractions.DTOs.Compute;

/// <summary>Serverless function deployment package reference.</summary>
public sealed record FunctionDeploymentRequest(
    string Name,
    string Runtime,
    string PackageUri,
    string? Handler = null,
    IReadOnlyDictionary<string, string>? EnvironmentVariables = null);
