using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArch.Workers.Messaging;

/// <summary>
/// Always-on worker — drains the integration-event subscription.
/// Where a Functions deployment would use a queue trigger, a
/// long-running worker polls/subscribes directly.
/// </summary>
public sealed class EventProcessorWorker(
    ILogger<EventProcessorWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Subscription loop — provider messaging client
                // delivers messages here; each is dispatched to
                // the same use cases the HTTP API calls.
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Event processing cycle failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
