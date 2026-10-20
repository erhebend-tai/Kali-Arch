using System.Security.Cryptography;
using System.Text;
using VirtualMachineService.Application.Interfaces.Webhooks;

namespace VirtualMachineService.Infrastructure.Webhooks;

public sealed class WebhookVerifier : IWebhookVerifier
{
    public string GenerateSignature(string payload, string secret)
    {
        var keyBytes     = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        var hash = HMACSHA256.HashData(keyBytes, payloadBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool VerifySignature(
        string payload, string signature, string secret)
    {
        // Strip prefix if present (e.g. "sha256=abc123")
        var cleanSignature = signature.StartsWith("sha256=")
            ? signature[7..]
            : signature;

        var expected = GenerateSignature(payload, secret);

        // Constant-time comparison — prevents timing attacks
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(cleanSignature));
    }
}
