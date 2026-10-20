using VirtualMachineService.Application.Webhooks;

namespace VirtualMachineService.Application.Interfaces.Webhooks;

public interface IWebhookRegistry
{
    Task<WebhookSubscription> RegisterAsync(
        RegisterWebhookRequest request,
        CancellationToken ct = default);

    Task UnregisterAsync(
        Guid subscriptionId,
        CancellationToken ct = default);

    Task<IEnumerable<WebhookSubscription>> GetSubscriptionsAsync(
        string eventType,
        string? tenantId = null,
        CancellationToken ct = default);

    Task<WebhookSubscription?> GetByIdAsync(
        Guid subscriptionId,
        CancellationToken ct = default);

    Task UpdateAsync(
        WebhookSubscription subscription,
        CancellationToken ct = default);
}
