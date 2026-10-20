namespace CleanArch.Shared.Abstractions.DTOs.Notifications;

public sealed record NotificationRequest(
    string Channel,
    string Title,
    string Body,
    NotificationSeverity Severity = NotificationSeverity.Info);
