namespace CleanArch.Shared.Abstractions.Application.Messaging;

/// <summary>
/// Stream analytics - Event Hubs / Kinesis / Kafka /
/// Cloudflare Queues consumers.
/// </summary>
public interface IStreamProcessingService
{
    Task StartStreamAsync(
        string streamName, CancellationToken ct = default);

    Task StopStreamAsync(
        string streamName, CancellationToken ct = default);

    Task<long> GetLagAsync(
        string streamName, string consumerGroup,
        CancellationToken ct = default);
}
