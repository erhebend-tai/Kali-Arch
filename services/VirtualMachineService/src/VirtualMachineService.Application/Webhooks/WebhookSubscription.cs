namespace VirtualMachineService.Application.Webhooks;

public sealed class WebhookSubscription
{
    public Guid     Id          { get; init; } = Guid.NewGuid();
    public string   TenantId    { get; init; } = string.Empty;
    public string   TargetUrl   { get; init; } = string.Empty;
    public string   Secret      { get; init; } = string.Empty;
    public string[] EventTypes  { get; init; } = [];
    public bool     IsActive    { get; set;  } = true;
    public int      MaxRetries  { get; init; } = 5;
    public DateTime CreatedAt   { get; init; } = DateTime.UtcNow;
    public DateTime? LastTriggeredAt { get; set; }
    public int      FailureCount    { get; set; }
}
