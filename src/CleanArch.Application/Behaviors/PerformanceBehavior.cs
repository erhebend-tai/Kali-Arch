using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
    int slowRequestThresholdMs = 500)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly TimeSpan _threshold =
        TimeSpan.FromMilliseconds(slowRequestThresholdMs);

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        if (stopwatch.Elapsed > _threshold)
            logger.LogWarning(
                "[SLOW REQUEST] {RequestName} took {ElapsedMs}ms (threshold {ThresholdMs}ms)",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds,
                _threshold.TotalMilliseconds);

        return response;
    }
}
