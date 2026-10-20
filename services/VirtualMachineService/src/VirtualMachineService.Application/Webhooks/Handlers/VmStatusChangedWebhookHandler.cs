using VirtualMachineService.Application.Common;
using VirtualMachineService.Application.Interfaces.Webhooks;
using VirtualMachineService.Application.Notifications;

namespace VirtualMachineService.Application.Webhooks.Handlers;

/// <summary>
/// Fans a vm.status-changed event out to all webhook subscribers —
/// queued through the outbox for at-least-once delivery.
/// </summary>
public sealed class VmStatusChangedWebhookHandler(
    IWebhookService webhookService)
    : INotificationHandler<VmStatusChangedNotification>
{
    public Task Handle(
        VmStatusChangedNotification notification,
        CancellationToken ct) =>
        webhookService.QueueAsync(
            "vm.status-changed",
            new
            {
                virtualMachineId = notification.VmId,
                name             = notification.VmName,
                oldStatus        = notification.OldStatus,
                newStatus        = notification.NewStatus,
                occurredAt       = DateTime.UtcNow
            },
            notification.TenantId,
            ct);
}
