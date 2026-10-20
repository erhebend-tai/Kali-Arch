using CleanArch.Shared.Grpc.Abstractions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace VirtualMachineService.API.Grpc.Interceptors;

/// <summary>
/// Server-side — validates JWT on every inbound call and stashes
/// the user on UserState["CurrentUser"] for downstream use.
/// </summary>
public sealed class AuthInterceptor(
    ITokenService tokenService,
    ILogger<AuthInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest                               request,
        ServerCallContext                      context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        await ValidateTokenAsync(context);
        return await continuation(request, context);
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest                                         request,
        IServerStreamWriter<TResponse>                   responseStream,
        ServerCallContext                                context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await ValidateTokenAsync(context);
        await continuation(request, responseStream, context);
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest>                     requestStream,
        ServerCallContext                                context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await ValidateTokenAsync(context);
        return await continuation(requestStream, context);
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest>                     requestStream,
        IServerStreamWriter<TResponse>                   responseStream,
        ServerCallContext                                context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await ValidateTokenAsync(context);
        await continuation(requestStream, responseStream, context);
    }

    private async Task ValidateTokenAsync(ServerCallContext context)
    {
        var authHeader = context.RequestHeaders
            .FirstOrDefault(h => h.Key == "authorization");

        if (authHeader is null)
            throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                "Authorization header missing."));

        var token = authHeader.Value.Replace("Bearer ", "");
        var user  = await tokenService.ValidateAccessTokenAsync(
            token, context.CancellationToken);

        if (user is null)
            throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                "Invalid or expired token."));

        // Add user info to context for downstream use
        context.UserState["CurrentUser"] = user;
    }
}
