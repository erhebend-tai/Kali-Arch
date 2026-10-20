using VirtualMachineService.Application.Common;
using VirtualMachineService.Application.Interfaces.Webhooks;
using VirtualMachineService.Application.Notifications;

namespace VirtualMachineService.Application.Webhooks.Handlers;

/// <summary>
/// Fans a vm.provisioned event out to all webhook subscribers —
/// queued through the outbox for at-least-once delivery.
/// </summary>
public sealed class VmProvisionedWebhookHandler(
    IWebhookService webhookService)
    : INotificationHandler<VmProvisionedNotification>
{
    public Task Handle(
        VmProvisionedNotification notification,
        CancellationToken ct) =>
        webhookService.QueueAsync(
            "vm.provisioned",
            new
            {
                virtualMachineId = notification.VmId,
                name             = notification.VmName,
                region           = notification.Region,
                size             = notification.Size,
                provisionedBy    = notification.ProvisionedBy,
                occurredAt       = DateTime.UtcNow
            },
            notification.TenantId,
            ct);
}
