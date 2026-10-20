using MediatR;

namespace CleanArch.Application.FeatureFlags;

/// <summary>
/// Requests implementing IFeatureFlaggedRequest declare the flag
/// they need — disabled flag → the request never reaches the handler.
/// </summary>
public interface IFeatureFlaggedRequest
{
    string RequiredFlag { get; }
}

public sealed class FeatureFlagBehavior<TRequest, TResponse>(
    IFeatureFlagService featureFlags)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IFeatureFlaggedRequest
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var enabled = await featureFlags.IsEnabledAsync(
            request.RequiredFlag, ct);

        if (!enabled)
            throw new InvalidOperationException(
                $"Feature '{request.RequiredFlag}' is not enabled.");

        return await next();
    }
}
