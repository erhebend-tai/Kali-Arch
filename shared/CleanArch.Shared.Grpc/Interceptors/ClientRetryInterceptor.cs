using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace CleanArch.Shared.Grpc.Interceptors;

/// <summary>
/// Client-side interceptor — retries unary calls on transient
/// failures (Unavailable / DeadlineExceeded) with linear backoff.
/// </summary>
public sealed class ClientRetryInterceptor(
    ILogger<ClientRetryInterceptor> logger) : Interceptor
{
    private const int MaxRetries = 3;
    private static readonly TimeSpan BaseDelay = TimeSpan.FromMilliseconds(200);

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            RetryAsync(request, context, continuation, call.ResponseAsync),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    private async Task<TResponse> RetryAsync<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation,
        Task<TResponse> response)
        where TRequest : class
        where TResponse : class
    {
        var attempt = 0;

        while (true)
        {
            try
            {
                return await response.ConfigureAwait(false);
            }
            catch (RpcException ex)
                when (IsTransient(ex.StatusCode) && attempt < MaxRetries)
            {
                attempt++;
                var delay = BaseDelay * attempt;

                logger.LogWarning(
                    "gRPC call {Method} failed transiently " +
                    "({StatusCode}); retry {Attempt}/{MaxRetries} in {Delay}ms",
                    context.Method.Name,
                    ex.StatusCode, attempt, MaxRetries, delay.TotalMilliseconds);

                await Task.Delay(delay, context.Options.CancellationToken)
                    .ConfigureAwait(false);

                response = continuation(request, context).ResponseAsync;
            }
        }
    }

    private static bool IsTransient(StatusCode statusCode) =>
        statusCode is StatusCode.Unavailable
                   or StatusCode.DeadlineExceeded
                   or StatusCode.ResourceExhausted;
}
