using CleanArch.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArch.FileWatcher.Handlers;

/// <summary>
/// Uploads changed files to object storage — the on-disk
/// directory becomes a drop folder for the configured container.
/// </summary>
public sealed class BlobSyncHandler(
    IObjectStorageService storageService,
    ILogger<BlobSyncHandler> logger,
    string container) : IFileChangedHandler
{
    public string FileFilter => "*.*";

    public async Task HandleAsync(
        FileChangedEventArgs args, CancellationToken ct = default)
    {
        if (args.ChangeKind == FileChangeKind.Deleted)
        {
            await storageService.DeleteAsync(container, args.Name, ct);
            return;
        }

        await using var stream = File.OpenRead(args.FullPath);
        var url = await storageService.UploadAsync(
            container, args.Name, stream,
            "application/octet-stream", ct);

        logger.LogInformation("Synced {File} → {Url}", args.Name, url);
    }
}
