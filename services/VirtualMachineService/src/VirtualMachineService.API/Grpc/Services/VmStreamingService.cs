using System.Reactive.Linq;
using System.Threading.Channels;
using Grpc.Core;
using VirtualMachineService.Application.Reactive.Streams;

namespace VirtualMachineService.API.Grpc.Services;

/// <summary>
/// Streaming RPCs — server-streaming status/metrics feeds and
/// the bidirectional console channel.
/// </summary>
public sealed partial class VmGrpcService
{
    // ── Server Streaming — live status transitions ────────────
    public override async Task StreamVmStatus(
        GetVmRequest                        request,
        IServerStreamWriter<VmStatusUpdate> responseStream,
        ServerCallContext                   context)
    {
        var vmId = Guid.Parse(request.VmId);

        logger.LogInformation(
            "Client subscribed to VM status stream for {VmId}", vmId);

        // Pump the hot observable through a channel so the
        // response stream stays strictly ordered.
        var channel = Channel.CreateUnbounded<VmStatusChanged>();

        using var subscription = statusStream.Changes
            .Where(c => c.VmId == vmId)
            .Subscribe(c => channel.Writer.TryWrite(c));

        await foreach (var update in channel.Reader
            .ReadAllAsync(context.CancellationToken))
        {
            await responseStream.WriteAsync(new VmStatusUpdate
            {
                VmId      = update.VmId.ToString(),
                OldStatus = update.OldStatus
                    .ToString().ToLowerInvariant(),
                NewStatus = update.NewStatus
                    .ToString().ToLowerInvariant(),
                ChangedAt = ToTimestamp(update.ChangedAt)
            });
        }
    }

    // ── Server Streaming — live metrics ───────────────────────
    public override async Task StreamVmMetrics(
        StreamMetricsRequest                 request,
        IServerStreamWriter<VmMetricsUpdate> responseStream,
        ServerCallContext                    context)
    {
        var interval = request.IntervalSeconds > 0
            ? TimeSpan.FromSeconds(request.IntervalSeconds)
            : TimeSpan.FromSeconds(5);

        // Simulated sampler — swap for the real metrics source.
        var random = new Random();

        while (!context.CancellationToken.IsCancellationRequested)
        {
            await responseStream.WriteAsync(new VmMetricsUpdate
            {
                VmId          = request.VmId,
                CpuPercent    = random.NextDouble() * 100,
                MemoryMb      = random.Next(256, 8192),
                DiskReadMbps  = random.NextDouble() * 500,
                DiskWriteMbps = random.NextDouble() * 500,
                NetInMbps     = random.NextDouble() * 1000,
                NetOutMbps    = random.NextDouble() * 1000,
                SampledAt     = ToTimestamp(DateTime.UtcNow)
            });

            await Task.Delay(interval, context.CancellationToken);
        }
    }

    // ── Bidirectional Streaming — VM Console ──────────────────
    public override async Task VmConsole(
        IAsyncStreamReader<ConsoleInput>   requestStream,
        IServerStreamWriter<ConsoleOutput> responseStream,
        ServerCallContext                  context)
    {
        await foreach (var input in requestStream.ReadAllAsync(
            context.CancellationToken))
        {
            // Echo back with simulated response
            await responseStream.WriteAsync(new ConsoleOutput
            {
                Data    = $"$ {input.Data}\n",
                IsError = false
            });
        }
    }
}
