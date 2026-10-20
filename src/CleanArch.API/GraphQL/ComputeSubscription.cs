using System.Runtime.CompilerServices;
using CleanArch.Application.Reactive.Streams;

namespace CleanArch.API.GraphQL;

/// <summary>
/// GraphQL subscription — wraps the VmStatusStream Rx
/// observable as an async enumerable over WebSocket.
/// </summary>
public sealed class ComputeSubscription
{
    [Subscribe]
    public VmStatusChanged VmStatusChanged(
        [EventMessage] VmStatusChanged change) => change;

    /// <summary>
    /// Bridges the application-layer stream into the GraphQL
    /// subscription pipeline — HotChocolate picks up
    /// IObservable directly via [SubscribeAndResolve].
    /// </summary>
    public IAsyncEnumerable<VmStatusChanged> StreamVmStatus(
        [Service] VmStatusStream stream,
        CancellationToken ct) =>
        Enumerate(stream, ct);

    private static async IAsyncEnumerable<VmStatusChanged> Enumerate(
        VmStatusStream stream,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var channel = System.Threading.Channels.Channel
            .CreateUnbounded<VmStatusChanged>();

        using var subscription = stream.Changes.Subscribe(
            change => channel.Writer.TryWrite(change));

        await foreach (var change in
            channel.Reader.ReadAllAsync(ct))
        {
            yield return change;
        }
    }
}
