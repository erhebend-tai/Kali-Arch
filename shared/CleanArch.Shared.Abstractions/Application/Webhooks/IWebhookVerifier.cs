namespace CleanArch.Shared.Abstractions.Application.Webhooks;

/// <summary>
/// Inbound webhook signature verification - HMAC validation
/// for GitHub / Stripe / provider callbacks.
/// </summary>
public interface IWebhookVerifier
{
    Task<bool> VerifyAsync(
        byte[] payload, string signature, string secret,
        CancellationToken ct = default);
}
