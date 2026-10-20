using CleanArch.Application.Reactive.Streams;
using Microsoft.AspNetCore.SignalR;

namespace CleanArch.API.Hubs;

/// <summary>
/// Bridges the application-layer VmStatusStream (Rx) to
/// SignalR — status changes fan out to "vms" and the
/// per-VM group without the hub knowing about domain types.
/// </summary>
public sealed class VmStatusBroadcaster(
    IHubContext<VmStatusHub> hub,
    VmStatusStream stream,
    ILogger<VmStatusBroadcaster> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var tcs = new TaskCompletionSource();
        await using var registration =
            stoppingToken.Register(() => tcs.TrySetResult());

        using var subscription = stream.Changes.Subscribe(
            change =>
            {
                _ = Task.Run(async () =>
                {
                    await hub.Clients.Group("vms")
                        .SendAsync("vmStatusChanged", change,
                            stoppingToken);
                    await hub.Clients.Group($"vm-{change.VmId}")
                        .SendAsync("vmStatusChanged", change,
                            stoppingToken);
                }, stoppingToken);
            });

        logger.LogInformation(
            "VmStatusBroadcaster started — bridging stream to SignalR.");

        await tcs.Task;
    }
}
