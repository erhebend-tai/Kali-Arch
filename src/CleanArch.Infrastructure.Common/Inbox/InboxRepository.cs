using CleanArch.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Common.Inbox;

public interface IInboxRepository
{
    /// <summary>True if this event was already received.</summary>
    Task<bool> ExistsAsync(string eventId, CancellationToken ct);

    Task AddAsync(InboxMessage message, CancellationToken ct);

    Task UpdateAsync(InboxMessage message, CancellationToken ct);
}

public sealed class InboxRepository(AppDbContext context)
    : IInboxRepository
{
    public async Task<bool> ExistsAsync(
        string eventId, CancellationToken ct) =>
        await context.InboxMessages
            .AnyAsync(m => m.EventId == eventId, ct);

    public async Task AddAsync(
        InboxMessage message, CancellationToken ct) =>
        await context.InboxMessages.AddAsync(message, ct);

    public Task UpdateAsync(
        InboxMessage message, CancellationToken ct)
    {
        context.InboxMessages.Update(message);
        return Task.CompletedTask;
    }
}
