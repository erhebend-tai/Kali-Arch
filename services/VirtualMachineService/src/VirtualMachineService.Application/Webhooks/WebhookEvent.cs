namespace VirtualMachineService.Application.Webhooks;

public sealed record WebhookEvent<T>(
    Guid     EventId,
    string   EventType,
    string   ApiVersion,
    DateTime OccurredAt,
    T        Data,
    string?  TenantId = null)
{
    public static WebhookEvent<T> Create(string eventType, T data,
        string? tenantId = null) =>
        new(
            EventId:    Guid.NewGuid(),
            EventType:  eventType,
            ApiVersion: "2024-01-01",
            OccurredAt: DateTime.UtcNow,
            Data:       data,
            TenantId:   tenantId);
}
