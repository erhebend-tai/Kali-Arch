using CleanArch.Application.Notifications;
using CleanArch.Domain.Events.Compute;
using CleanArch.Shared.Domain.Interfaces;
using MediatR;

namespace CleanArch.Infrastructure.Common.Persistence;

/// <summary>
/// Domain event → MediatR notification. Add one case per event type —
/// keeps the coupling between UoW and Application in one place.
/// </summary>
public static class DomainEventMapper
{
    public static INotification? MapToNotification(
        IDomainEvent domainEvent) => domainEvent switch
    {
        VmProvisionedEvent e => new VmProvisionedNotification(e),
        _ => null
    };
}
