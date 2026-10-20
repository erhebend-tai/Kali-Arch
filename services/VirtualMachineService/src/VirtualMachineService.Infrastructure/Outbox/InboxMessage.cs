namespace VirtualMachineService.Infrastructure.Outbox;

/// <summary>
/// Inbox row — mirrors outbox.inbox_messages. message_id is the
/// idempotency key: consumers insert first, process second.
/// </summary>
public sealed class InboxMessage
{
    public Guid      Id           { get; init; } = Guid.NewGuid();
    public string    MessageId    { get; init; } = string.Empty;
    public string    Topic        { get; init; } = string.Empty;
    public string    PayloadType  { get; init; } = string.Empty;
    public string    Payload      { get; init; } = string.Empty;
    public DateTime  ReceivedAt   { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedAt  { get; set; }
    public int       RetryCount   { get; set; }
    public string?   Error        { get; set; }
    public bool      IsDuplicate  { get; set; }
}
