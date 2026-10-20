namespace CleanArch.Shared.Abstractions.Application.Storage;

/// <summary>
/// Analytical store - Data Lake / BigQuery / Synapse / Redshift.
/// </summary>
public interface IDataWarehouseService
{
    Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql, CancellationToken ct = default);

    Task IngestAsync<T>(
        string dataset, string table,
        IEnumerable<T> rows, CancellationToken ct = default);
}
