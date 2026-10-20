namespace CleanArch.Shared.Abstractions.Application.Messaging;

/// <summary>
/// Cloud-neutral event bus contract - implemented per provider
/// (Service Bus / SQS+SNS / PubSub / OSS MNS / OCI Streaming / CF Queues).
/// </summary>
public interface IMessageBusService
{
    Task PublishAsync<T>(
        string topic,
        T message,
        CancellationToken ct = default);

    Task PublishBatchAsync<T>(
        string topic,
        IEnumerable<T> messages,
        CancellationToken ct = default);

    Task SubscribeAsync<T>(
        string topic,
        Func<T, CancellationToken, Task> handler,
        CancellationToken ct = default);
}
