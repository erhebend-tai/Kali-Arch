using CleanArch.Shared.Domain.Primitives;

namespace CleanArch.Infrastructure.Common.Inbox;

/// <summary>
/// Consume-side of the Outbox — deduplicates delivered
/// integration events so handlers are idempotent.
/// </summary>
public sealed class InboxMessage : AuditableEntity
{
    public required string EventType { get; init; }
    public required string EventId { get; init; }
    public required string Payload { get; init; }
    public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; private set; }
    public string? Error { get; private set; }
    public int AttemptCount { get; private set; }

    public bool IsProcessed => ProcessedAt is not null;

    public void MarkProcessed() => ProcessedAt = DateTime.UtcNow;

    public void MarkFailed(string error)
    {
        Error = error;
        AttemptCount++;
    }
}
