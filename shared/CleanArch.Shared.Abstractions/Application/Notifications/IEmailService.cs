using CleanArch.Shared.Abstractions.DTOs.Notifications;

namespace CleanArch.Shared.Abstractions.Application.Notifications;

/// <summary>
/// Email sending - SendGrid / SES / SMTP.
/// </summary>
public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);

    Task SendTemplatedAsync(
        string templateId, string to,
        IReadOnlyDictionary<string, string> variables,
        CancellationToken ct = default);
}
