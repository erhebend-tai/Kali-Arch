using CleanArch.Shared.Abstractions.DTOs.Notifications;

namespace CleanArch.Shared.Abstractions.Application.Notifications;

/// <summary>
/// Ops notifications - Slack / Teams / PagerDuty / webhooks.
/// </summary>
public interface INotificationService
{
    Task SendAsync(NotificationRequest request,
        CancellationToken ct = default);
}
