namespace CleanArch.Shared.Abstractions.Application.Notifications;

/// <summary>SMS sending - Twilio / SNS / provider SMS APIs.</summary>
public interface ISmsService
{
    Task SendAsync(string to, string body, CancellationToken ct = default);
}
