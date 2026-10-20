namespace VirtualMachineService.Application.Interfaces.Webhooks;

public interface IWebhookService
{
    /// <summary>Dispatch event to all registered subscribers.</summary>
    Task DispatchAsync<T>(
        string eventType,
        T payload,
        string? tenantId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Queue for reliable delivery (outbox pattern) —
    /// at-least-once with retry via WebhookDeliveryWorker.
    /// </summary>
    Task QueueAsync<T>(
        string eventType,
        T payload,
        string? tenantId = null,
        CancellationToken ct = default);
}
