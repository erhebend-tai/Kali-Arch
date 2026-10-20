using CleanArch.Application.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CleanArch.Infrastructure.Common.Integrations.SendGrid;

/// <summary>SendGrid implementation of IEmailService.</summary>
public sealed class SendGridEmailService(
    SendGridClient client, string fromAddress)
    : IEmailService
{
    public async Task SendAsync(
        EmailMessage message, CancellationToken ct = default)
    {
        var mail = MailHelper.CreateSingleEmail(
            new EmailAddress(fromAddress, "CleanArch Platform"),
            new EmailAddress(message.To),
            message.Subject,
            message.PlainTextBody,
            message.HtmlBody);

        await client.SendEmailAsync(mail, ct);
    }
}
