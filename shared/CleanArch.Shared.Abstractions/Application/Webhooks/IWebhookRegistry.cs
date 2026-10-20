using CleanArch.Shared.Abstractions.DTOs.Webhooks;

namespace CleanArch.Shared.Abstractions.Application.Webhooks;

/// <summary>Store of active webhook subscriptions.</summary>
public interface IWebhookRegistry
{
    Task<IReadOnlyList<WebhookSubscription>> ListAsync(
        string? eventType = null, CancellationToken ct = default);

    Task<WebhookSubscription?> GetAsync(
        string subscriptionId, CancellationToken ct = default);
}
