using CleanArch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Behaviors;

/// <summary>
/// Opt-in query caching — any request implementing ICacheableQuery
/// gets cache-read-through behavior.
/// </summary>
public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? CacheDuration { get; }
}

public sealed class CachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var cached = await cacheService.GetAsync<TResponse>(
            request.CacheKey, ct);

        if (cached is not null)
        {
            logger.LogInformation("[CACHE HIT] {CacheKey}", request.CacheKey);
            return cached;
        }

        var response = await next();

        await cacheService.SetAsync(
            request.CacheKey, response, request.CacheDuration, ct);
        return response;
    }
}
