namespace CleanArch.Infrastructure.Common.Outbox;

/// <summary>
/// Transactional outbox — integration messages are written in the
/// same transaction as business data, then relayed by OutboxProcessor.
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }
}
