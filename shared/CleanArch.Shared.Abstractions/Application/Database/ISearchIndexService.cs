namespace CleanArch.Shared.Abstractions.Application.Database;

/// <summary>
/// Search index management - AI Search / Elasticsearch /
/// OpenSearch / Algolia.
/// </summary>
public interface ISearchIndexService
{
    Task CreateIndexAsync(
        string indexName, CancellationToken ct = default);

    Task DeleteIndexAsync(
        string indexName, CancellationToken ct = default);

    Task<bool> IndexExistsAsync(
        string indexName, CancellationToken ct = default);
}
