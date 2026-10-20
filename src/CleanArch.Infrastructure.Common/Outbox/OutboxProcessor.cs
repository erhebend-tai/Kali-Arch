using System.Text.Json;
using CleanArch.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Outbox;

/// <summary>
/// Background relay — polls pending outbox messages and publishes
/// them to the message bus (at-least-once delivery).
/// </summary>
public sealed class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var repository =
                    scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var bus =
                    scope.ServiceProvider.GetRequiredService<IMessageBusService>();

                var pending = await repository.GetPendingAsync(10, stoppingToken);

                foreach (var message in pending)
                {
                    try
                    {
                        var payload = JsonSerializer.Deserialize<object>(
                            message.Content);
                        await bus.PublishAsync("integration-events",
                            payload, stoppingToken);
                        await repository.MarkProcessedAsync(
                            message.Id, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        await repository.MarkFailedAsync(
                            message.Id, ex.Message, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox processing error");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
