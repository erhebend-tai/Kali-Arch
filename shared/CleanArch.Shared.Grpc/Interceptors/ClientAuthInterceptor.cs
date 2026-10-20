using CleanArch.Shared.Grpc.Abstractions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace CleanArch.Shared.Grpc.Interceptors;

/// <summary>
/// Client-side interceptor — mints a service-account token and
/// attaches it to every outbound gRPC call.
/// </summary>
public sealed class ClientAuthInterceptor(ITokenService tokenService)
    : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var callOptions = AddAuthHeader(context.Options);
        var newContext  = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method, context.Host, callOptions);

        return continuation(request, newContext);
    }

    private CallOptions AddAuthHeader(CallOptions options)
    {
        var token = tokenService.GenerateAccessToken(
            new UserInfo(
                "service-account",
                "svc@platform.internal",
                []));

        var metadata = options.Headers ?? new Metadata();
        metadata.Add("authorization", $"Bearer {token}");

        return options.WithHeaders(metadata);
    }
}
