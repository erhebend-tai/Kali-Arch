using CleanArch.Application.Commands.Compute;
using CleanArch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Chains.Compute;

public sealed class CacheCheckHandler(
    ICacheService cacheService,
    ILogger<CacheCheckHandler> logger)
    : RequestChainHandler<ProvisionVmCommand, ProvisionVmResult>
{
    protected override async Task<bool> CanHandleAsync(
        ProvisionVmCommand request, CancellationToken ct)
    {
        var cached = await cacheService.GetAsync<ProvisionVmResult>(
            $"vm-provisioned-{request.Name}", ct);

        return cached is not null;
    }

    protected override Task<ProvisionVmResult> HandleInternalAsync(
        ProvisionVmCommand request, CancellationToken ct)
    {
        logger.LogInformation("Cache hit — short-circuiting provisioning for {Name}", request.Name);
        return cacheService.GetAsync<ProvisionVmResult>(
            $"vm-provisioned-{request.Name}", ct)!;
    }
}

public sealed class RateLimitHandler(IRateLimiter rateLimiter)
    : RequestChainHandler<ProvisionVmCommand, ProvisionVmResult>
{
    protected override async Task<bool> CanHandleAsync(
        ProvisionVmCommand request, CancellationToken ct)
    {
        var result = await rateLimiter.CheckAsync(
            request.Name, 5, TimeSpan.FromMinutes(1), ct);

        if (!result.Allowed)
            throw new InvalidOperationException(
                $"Rate limit exceeded. Retry after {result.RetryAfter}.");

        return false; // pass to next handler — never handles itself
    }

    protected override Task<ProvisionVmResult> HandleInternalAsync(
        ProvisionVmCommand request, CancellationToken ct) =>
        throw new NotImplementedException();
}

public sealed class ProvisionVmChainHandler(ISender sender)
    : RequestChainHandler<ProvisionVmCommand, ProvisionVmResult>
{
    protected override Task<bool> CanHandleAsync(
        ProvisionVmCommand request, CancellationToken ct) =>
        Task.FromResult(true); // always the terminal handler

    protected override Task<ProvisionVmResult> HandleInternalAsync(
        ProvisionVmCommand request, CancellationToken ct) =>
        sender.Send(request, ct); // delegate to MediatR pipeline
}
