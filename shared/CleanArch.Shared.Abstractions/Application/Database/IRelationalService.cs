namespace CleanArch.Shared.Abstractions.Application.Database;

/// <summary>
/// Generic relational access - SQL Server / Postgres / MySQL.
/// Dapper-style; EF Core stays behind repository contracts.
/// </summary>
public interface IRelationalService
{
    Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql, object? parameters = null,
        CancellationToken ct = default);

    Task<T?> QuerySingleAsync<T>(
        string sql, object? parameters = null,
        CancellationToken ct = default);

    Task<int> ExecuteAsync(
        string sql, object? parameters = null,
        CancellationToken ct = default);
}
