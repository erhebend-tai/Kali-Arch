using MediatR;

namespace VirtualMachineService.Application.Notifications;

/// <summary>Published after a VM is provisioned — fans out to webhooks.</summary>
public sealed record VmProvisionedNotification(
    Guid   VmId,
    string VmName,
    string Region,
    string Size,
    string TenantId,
    string ProvisionedBy) : INotification;

/// <summary>Published on any VM status transition.</summary>
public sealed record VmStatusChangedNotification(
    Guid   VmId,
    string VmName,
    string OldStatus,
    string NewStatus,
    string TenantId) : INotification;
