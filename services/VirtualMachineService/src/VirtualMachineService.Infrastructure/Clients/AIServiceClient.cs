using CleanArch.Shared.Grpc.Ai;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace VirtualMachineService.Infrastructure.Clients;

/// <summary>Typed gRPC client — wraps the generated AIService stub.</summary>
public sealed class AIServiceClient(
    AIService.AIServiceClient grpcClient,
    ILogger<AIServiceClient> logger)
{
    public async Task<string?> GenerateCompletionAsync(
        string prompt, string tenantId,
        string? model = null,
        CancellationToken ct = default)
    {
        try
        {
            var response = await grpcClient.GenerateCompletionAsync(
                new CompletionRequest
                {
                    Prompt   = prompt,
                    Model    = model ?? string.Empty,
                    TenantId = tenantId
                },
                cancellationToken: ct);

            return response.Text;
        }
        catch (RpcException ex)
            when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
        catch (RpcException ex)
        {
            logger.LogError(ex,
                "AIService.GenerateCompletion failed: {StatusCode}",
                ex.StatusCode);
            throw;
        }
    }
}
