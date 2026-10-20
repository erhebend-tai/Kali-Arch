namespace CleanArch.Shared.Abstractions.Application.Database;

/// <summary>
/// Document database - Cosmos DB / DynamoDB / Firestore /
/// Table Storage / Mongo-compatible stores.
/// </summary>
public interface INoSqlService
{
    Task<T?> GetAsync<T>(
        string container, string id,
        CancellationToken ct = default);

    Task UpsertAsync<T>(
        string container, string id, T document,
        CancellationToken ct = default);

    Task DeleteAsync(
        string container, string id,
        CancellationToken ct = default);

    Task<IReadOnlyList<T>> QueryAsync<T>(
        string container, string query,
        CancellationToken ct = default);
}
