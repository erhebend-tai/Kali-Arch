namespace CleanArch.Shared.Abstractions.Application.Reports;

/// <summary>
/// Where generated reports land - object storage, database,
/// or local filesystem in dev.
/// </summary>
public interface IReportStorage
{
    /// <summary>Saves the report and returns its storage id.</summary>
    Task<string> SaveAsync(
        string reportName, byte[] content,
        string format, CancellationToken ct = default);

    Task<Stream> LoadAsync(
        string reportId, CancellationToken ct = default);

    Task DeleteAsync(
        string reportId, CancellationToken ct = default);
}
