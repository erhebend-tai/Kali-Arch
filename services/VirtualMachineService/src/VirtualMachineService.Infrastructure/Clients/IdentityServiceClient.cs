using CleanArch.Shared.Grpc.Abstractions;
using CleanArch.Shared.Grpc.Identity;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace VirtualMachineService.Infrastructure.Clients;

/// <summary>Typed gRPC client — wraps the generated IdentityService stub.</summary>
public sealed class IdentityServiceClient(
    IdentityService.IdentityServiceClient grpcClient,
    ILogger<IdentityServiceClient> logger)
{
    public async Task<UserInfo?> GetUserAsync(
        Guid userId, string tenantId,
        CancellationToken ct = default)
    {
        try
        {
            var response = await grpcClient.GetUserAsync(
                new GetUserRequest
                {
                    UserId   = userId.ToString(),
                    TenantId = tenantId
                },
                cancellationToken: ct);

            return new UserInfo(
                response.UserId,
                response.Email,
                response.Roles.ToList());
        }
        catch (RpcException ex)
            when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
        catch (RpcException ex)
        {
            logger.LogError(ex,
                "IdentityService.GetUser failed: {StatusCode}",
                ex.StatusCode);
            throw;
        }
    }
}
