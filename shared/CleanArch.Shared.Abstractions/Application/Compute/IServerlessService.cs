using CleanArch.Shared.Abstractions.DTOs.Compute;

namespace CleanArch.Shared.Abstractions.Application.Compute;

/// <summary>
/// Serverless functions - Azure Functions / Lambda / Cloud Functions /
/// Cloudflare Workers.
/// </summary>
public interface IServerlessService
{
    Task<string> DeployFunctionAsync(
        FunctionDeploymentRequest request,
        CancellationToken ct = default);

    Task<string> InvokeAsync(
        string functionId, string? payload = null,
        CancellationToken ct = default);

    Task DeleteAsync(
        string functionId, CancellationToken ct = default);
}
