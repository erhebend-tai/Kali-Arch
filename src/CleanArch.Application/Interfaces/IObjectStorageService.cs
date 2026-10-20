namespace CleanArch.Application.Interfaces;

/// <summary>
/// Cloud-neutral object storage — implemented per provider
/// (Blob / S3 / GCS / OSS / OCI / R2 / filesystem).
/// </summary>
public interface IObjectStorageService
{
    Task<string> UploadAsync(
        string container,
        string blobName,
        Stream content,
        string contentType,
        CancellationToken ct = default);

    Task<Stream?> DownloadAsync(
        string container,
        string blobName,
        CancellationToken ct = default);

    Task DeleteAsync(
        string container,
        string blobName,
        CancellationToken ct = default);

    Task<IReadOnlyList<string>> ListAsync(
        string container,
        string? prefix = null,
        CancellationToken ct = default);
}
