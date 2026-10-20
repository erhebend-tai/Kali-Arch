namespace CleanArch.FileWatcher.Handlers;

/// <summary>
/// Reaction to a filesystem event — one handler per concern
/// (config reload, blob sync, content invalidation, ...).
/// </summary>
public interface IFileChangedHandler
{
    /// <summary>Glob-style filter — e.g. "*.json", "appsettings.*".</summary>
    string FileFilter { get; }

    Task HandleAsync(FileChangedEventArgs args,
        CancellationToken ct = default);
}

public sealed record FileChangedEventArgs(
    string FullPath,
    string Name,
    FileChangeKind ChangeKind,
    DateTime OccurredAt);

public enum FileChangeKind
{
    Created,
    Changed,
    Deleted,
    Renamed
}
