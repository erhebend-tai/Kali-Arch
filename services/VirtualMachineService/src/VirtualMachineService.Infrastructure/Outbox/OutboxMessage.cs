namespace VirtualMachineService.Infrastructure.Outbox;

/// <summary>
/// Outbox row — mirrors outbox.messages. Written in the same
/// transaction as the domain change; dispatched by a worker.
/// </summary>
public sealed class OutboxMessage
{
    public Guid      Id            { get; init; } = Guid.NewGuid();
    public string    Topic         { get; init; } = string.Empty;
    public string    PayloadType   { get; init; } = string.Empty;
    public string    Payload       { get; init; } = string.Empty;
    public string?   TenantId      { get; init; }
    public DateTime  CreatedAt     { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedAt   { get; set; }
    public int       RetryCount    { get; set; }
    public DateTime? NextRetryAt   { get; set; }
    public string?   Error         { get; set; }
    public string?   CorrelationId { get; init; }
}
