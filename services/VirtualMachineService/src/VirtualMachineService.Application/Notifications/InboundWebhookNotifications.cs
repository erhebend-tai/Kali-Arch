using MediatR;

namespace VirtualMachineService.Application.Notifications;

/// <summary>Inbound webhook from GitHub — verified, ready to process.</summary>
public sealed record GitHubWebhookReceivedNotification(
    string EventType,
    string Payload) : INotification;

/// <summary>Inbound webhook from Stripe — verified, ready to process.</summary>
public sealed record StripeWebhookReceivedNotification(
    string Payload) : INotification;

/// <summary>Inbound webhook from a configured custom source.</summary>
public sealed record GenericWebhookReceivedNotification(
    string Source,
    string Payload) : INotification;
