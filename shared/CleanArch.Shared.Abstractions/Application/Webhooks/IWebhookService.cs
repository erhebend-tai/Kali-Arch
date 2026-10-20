using CleanArch.Shared.Abstractions.DTOs.Webhooks;

namespace CleanArch.Shared.Abstractions.Application.Webhooks;

/// <summary>Outbound webhook delivery to subscriber endpoints.</summary>
public interface IWebhookService
{
    Task<WebhookSubscription> RegisterAsync(
        RegisterWebhookRequest request,
        CancellationToken ct = default);

    Task UnregisterAsync(
        string subscriptionId, CancellationToken ct = default);

    Task DeliverAsync(
        string subscriptionId, object payload,
        CancellationToken ct = default);
}
