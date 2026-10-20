using CleanArch.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Services;

/// <summary>
/// Dev/test fallback — logs instead of sending. Real providers
/// (SendGrid, SES, etc.) replace this via TryAdd in the
/// provider's DI extension.
/// </summary>
public sealed class LoggingEmailService(
    ILogger<LoggingEmailService> logger) : IEmailService
{
    public Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[Email → {To}] {Subject}", message.To, message.Subject);
        return Task.CompletedTask;
    }
}
