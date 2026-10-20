using CleanArch.Application.Interfaces;
using CleanArch.Shared.Contracts.Events.Compute;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Notifications;

// Multiple handlers — Observer pattern via MediatR notifications.
// Each handler is an independent subscriber to the domain event.

public sealed class SendVmProvisionedEmailHandler(
    IEmailService emailService,
    ILogger<SendVmProvisionedEmailHandler> logger)
    : INotificationHandler<VmProvisionedNotification>
{
    public async Task Handle(
        VmProvisionedNotification notification, CancellationToken ct)
    {
        logger.LogInformation("Sending welcome email for VM {VmName}",
            notification.DomainEvent.VmName);
        await emailService.SendAsync(
            new EmailMessage(
                To: "admin@yourplatform.dev",
                Subject: $"VM provisioned: {notification.DomainEvent.VmName}",
                PlainTextBody:
                    $"VM {notification.DomainEvent.VmName} " +
                    $"({notification.DomainEvent.VmId}) is ready."), ct);
    }
}

public sealed class UpdateDashboardMetricsHandler(
    IDashboardService dashboardService)
    : INotificationHandler<VmProvisionedNotification>
{
    public Task Handle(
        VmProvisionedNotification notification, CancellationToken ct) =>
        dashboardService.IncrementVmCountAsync(ct);
}

public sealed class PublishIntegrationEventHandler(
    IMessageBusService messageBusService,
    ILogger<PublishIntegrationEventHandler> logger)
    : INotificationHandler<VmProvisionedNotification>
{
    public async Task Handle(
        VmProvisionedNotification notification, CancellationToken ct)
    {
        logger.LogInformation("Publishing integration event for VM {VmName}",
            notification.DomainEvent.VmName);

        // Domain event → integration event for other services.
        var integrationEvent = new VmProvisionedIntegrationEvent(
            notification.DomainEvent.VmId,
            notification.DomainEvent.VmName,
            "unknown-region",
            "unknown-rg",
            notification.DomainEvent.OccurredAt);

        await messageBusService.PublishAsync(
            "vm.provisioned", integrationEvent, ct);
    }
}
