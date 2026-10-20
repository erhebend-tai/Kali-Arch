namespace CleanArch.Application.Interfaces.Notifications;

public interface INotificationService
{
    Task SendAsync(NotificationRequest request,
        CancellationToken ct = default);
}

public sealed record NotificationRequest(
    string Channel,
    string Title,
    string Body,
    NotificationSeverity Severity = NotificationSeverity.Info);

public enum NotificationSeverity { Info, Warning, Critical }
