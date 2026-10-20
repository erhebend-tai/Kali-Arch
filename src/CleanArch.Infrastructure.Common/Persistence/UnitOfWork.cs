using CleanArch.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Persistence;

/// <summary>
/// Transaction coordinator — all-or-nothing commits, domain events
/// dispatched through MediatR before the transaction completes.
/// </summary>
public class UnitOfWork(
    AppDbContext context,
    IPublisher publisher,
    ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction = await context.Database.BeginTransactionAsync(ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        await DispatchDomainEventsAsync(ct);
        return await context.SaveChangesAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await SaveChangesAsync(ct);
        if (_transaction is not null)
            await _transaction.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync(ct);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        context.Dispose();
    }

    private async Task DispatchDomainEventsAsync(CancellationToken ct)
    {
        var entities = context.EntitiesWithEvents;
        if (entities.Count == 0) return;

        foreach (var entity in entities)
        {
            foreach (var domainEvent in entity.DomainEvents)
            {
                logger.LogInformation("Dispatching domain event: {Event}",
                    domainEvent.GetType().Name);

                // Wrap the domain event in a MediatR notification —
                // NotificationMapper maps each event type to its wrapper.
                var notification =
                    DomainEventMapper.MapToNotification(domainEvent);
                if (notification is not null)
                    await publisher.Publish(notification, ct);
            }

            entity.ClearDomainEvents();
        }
    }
}
