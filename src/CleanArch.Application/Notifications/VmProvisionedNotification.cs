using CleanArch.Domain.Events.Compute;
using MediatR;

namespace CleanArch.Application.Notifications;

/// <summary>
/// Bridges domain events → MediatR notifications.
/// Raised by UnitOfWork.CommitAsync before the transaction completes.
/// </summary>
public sealed record VmProvisionedNotification(VmProvisionedEvent DomainEvent)
    : INotification;
