using System.Reactive.Linq;
using System.Reactive.Subjects;
using CleanArch.Domain.Enums.Compute;

namespace CleanArch.Application.Reactive.Streams;

/// <summary>
/// Hot observable — broadcasts VM status changes to all
/// subscribers. SignalR hubs, metrics, and audit listeners
/// subscribe rather than poll.
/// </summary>
public sealed class VmStatusStream : IDisposable
{
    private readonly Subject<VmStatusChanged> _subject = new();

    public IObservable<VmStatusChanged> Changes =>
        _subject.AsObservable();

    public void Push(VmStatusChanged change) =>
        _subject.OnNext(change);

    public IObservable<VmStatusChanged> RunningVms =>
        Changes.Where(c => c.NewStatus == VmStatus.Running);

    public IObservable<VmStatusChanged> StoppedVms =>
        Changes.Where(c => c.NewStatus == VmStatus.Stopped);

    /// <summary>At most one event per VM per 5 seconds.</summary>
    public IObservable<VmStatusChanged> ThrottledChanges =>
        Changes
            .GroupBy(c => c.VmId)
            .SelectMany(g => g.Throttle(TimeSpan.FromSeconds(5)));

    /// <summary>Batches events every 10 seconds.</summary>
    public IObservable<IList<VmStatusChanged>> BatchedChanges =>
        Changes.Buffer(TimeSpan.FromSeconds(10));

    public void Dispose() => _subject.Dispose();
}

public sealed record VmStatusChanged(
    Guid VmId,
    string VmName,
    VmStatus OldStatus,
    VmStatus NewStatus,
    DateTime ChangedAt);
