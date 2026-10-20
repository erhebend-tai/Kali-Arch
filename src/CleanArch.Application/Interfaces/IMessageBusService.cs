namespace CleanArch.Application.Interfaces;

/// <summary>
/// Cloud-neutral event bus contract — implemented per provider
/// (Service Bus / SQS+SNS / PubSub / OSS MNS / OCI Streaming / CF Queues).
/// </summary>
public interface IMessageBusService
{
    Task PublishAsync<T>(
        string topic,
        T message,
        CancellationToken ct = default);
}
