using CleanArch.FileWatcher.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArch.FileWatcher.Watchers;

/// <summary>
/// Always-on FileSystemWatcher hosted service — watches a directory
/// and fans each event out to all registered IFileChangedHandlers.
/// </summary>
public sealed class ConfigFileWatcher(
    IEnumerable<IFileChangedHandler> handlers,
    ILogger<ConfigFileWatcher> logger,
    string watchPath,
    string filter = "*.*") : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!Directory.Exists(watchPath))
        {
            logger.LogWarning("Watch path does not exist: {Path}", watchPath);
            return;
        }

        var tcs = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);

        using var watcher = new FileSystemWatcher(watchPath, filter)
        {
            NotifyFilter = NotifyFilters.FileName
                         | NotifyFilters.LastWrite
                         | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        void OnChanged(object sender, FileSystemEventArgs e) =>
            _ = DispatchAsync(MapKind(e.ChangeType), e.FullPath,
                e.Name ?? Path.GetFileName(e.FullPath));

        void OnRenamed(object sender, RenamedEventArgs e) =>
            _ = DispatchAsync(FileChangeKind.Renamed, e.FullPath,
                e.Name ?? Path.GetFileName(e.FullPath));

        watcher.Created += OnChanged;
        watcher.Changed += OnChanged;
        watcher.Deleted += OnChanged;
        watcher.Renamed += OnRenamed;

        logger.LogInformation("Watching {Path} for {Filter}", watchPath, filter);
        stoppingToken.Register(() => tcs.TrySetResult());
        await tcs.Task;

        return;

        static FileChangeKind MapKind(WatcherChangeTypes changeType) =>
            changeType switch
            {
                WatcherChangeTypes.Created => FileChangeKind.Created,
                WatcherChangeTypes.Changed => FileChangeKind.Changed,
                WatcherChangeTypes.Deleted => FileChangeKind.Deleted,
                _ => FileChangeKind.Changed
            };

        async Task DispatchAsync(
            FileChangeKind kind, string fullPath, string name)
        {
            var args = new FileChangedEventArgs(
                fullPath, name, kind, DateTime.UtcNow);

            foreach (var handler in handlers)
            {
                if (!MatchesFilter(name, handler.FileFilter)) continue;

                try { await handler.HandleAsync(args, stoppingToken); }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        "File-change handler {Handler} failed for {File}",
                        handler.GetType().Name, name);
                }
            }
        }

        static bool MatchesFilter(string fileName, string glob)
        {
            if (glob == "*.*" || glob == "*") return true;
            if (glob.StartsWith("*."))
                return fileName.EndsWith(glob[1..],
                    StringComparison.OrdinalIgnoreCase);
            return string.Equals(fileName, glob,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
