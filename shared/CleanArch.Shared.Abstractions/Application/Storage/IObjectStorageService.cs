namespace CleanArch.Shared.Abstractions.Application.Storage;

/// <summary>
/// Cloud-neutral object storage - implemented per provider
/// (Blob / S3 / GCS / OSS / OCI / R2 / filesystem).
/// </summary>
public interface IObjectStorageService
{
    Task<string> UploadAsync(
        string container,
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken ct = default);

    Task<Stream?> DownloadAsync(
        string container,
        string objectKey,
        CancellationToken ct = default);

    Task DeleteAsync(
        string container,
        string objectKey,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(
        string container,
        string objectKey,
        CancellationToken ct = default);

    Task<IReadOnlyList<string>> ListAsync(
        string container,
        string? prefix = null,
        CancellationToken ct = default);

    Task<string> GetPresignedUrlAsync(
        string container,
        string objectKey,
        TimeSpan expiry,
        CancellationToken ct = default);
}
