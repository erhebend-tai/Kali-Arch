namespace CleanArch.Shared.Abstractions.DTOs.Notifications;

public sealed record EmailMessage(
    string To,
    string Subject,
    string? PlainTextBody = null,
    string? HtmlBody = null);
