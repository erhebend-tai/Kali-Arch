using Grpc.Core;
using Grpc.Core.Interceptors;

namespace VirtualMachineService.API.Grpc.Interceptors;

/// <summary>
/// Server-side — resolves the caller's tenant from the
/// x-tenant-id metadata header and stashes it on
/// UserState["TenantId"].
/// </summary>
public sealed class TenantInterceptor(
    ILogger<TenantInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest                               request,
        ServerCallContext                      context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        ResolveTenant(context);
        return await continuation(request, context);
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest                                         request,
        IServerStreamWriter<TResponse>                   responseStream,
        ServerCallContext                                context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        ResolveTenant(context);
        await continuation(request, responseStream, context);
    }

    private void ResolveTenant(ServerCallContext context)
    {
        var tenantHeader = context.RequestHeaders
            .FirstOrDefault(h => h.Key == "x-tenant-id");

        if (tenantHeader is null)
        {
            logger.LogDebug(
                "No x-tenant-id header on {Method}", context.Method);
            return;
        }

        context.UserState["TenantId"] = tenantHeader.Value;
    }
}
