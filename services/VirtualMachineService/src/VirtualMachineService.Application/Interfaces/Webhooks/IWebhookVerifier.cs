namespace VirtualMachineService.Application.Interfaces.Webhooks;

public interface IWebhookVerifier
{
    /// <summary>Generate HMAC-SHA256 signature for outbound payload.</summary>
    string GenerateSignature(string payload, string secret);

    /// <summary>Verify inbound webhook signature.</summary>
    bool VerifySignature(
        string payload,
        string signature,
        string secret);
}
