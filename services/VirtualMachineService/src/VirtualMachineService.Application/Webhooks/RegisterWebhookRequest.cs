namespace VirtualMachineService.Application.Webhooks;

public sealed record RegisterWebhookRequest(
    string   TenantId,
    string   TargetUrl,
    string   Secret,
    string[] EventTypes,
    int      MaxRetries = 5);
