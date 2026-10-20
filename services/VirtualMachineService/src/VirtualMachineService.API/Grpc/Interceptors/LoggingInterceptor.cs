using Grpc.Core;
using Grpc.Core.Interceptors;

namespace VirtualMachineService.API.Grpc.Interceptors;

public sealed class LoggingInterceptor(
    ILogger<LoggingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest                               request,
        ServerCallContext                      context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var method = context.Method;
        var sw     = System.Diagnostics.Stopwatch.StartNew();

        logger.LogInformation(
            "gRPC call started: {Method}", method);

        try
        {
            var response = await continuation(request, context);
            sw.Stop();

            logger.LogInformation(
                "gRPC call completed: {Method} in {ElapsedMs}ms",
                method, sw.ElapsedMilliseconds);

            return response;
        }
        catch (RpcException ex)
        {
            sw.Stop();
            logger.LogWarning(
                "gRPC call failed: {Method} — {StatusCode}: {Detail}",
                method, ex.StatusCode, ex.Status.Detail);
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex,
                "gRPC call exception: {Method}", method);
            throw new RpcException(new Status(
                StatusCode.Internal, "An unexpected error occurred."));
        }
    }
}
