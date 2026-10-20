using CleanArch.Shared.Abstractions.DTOs.AI;

namespace CleanArch.Shared.Abstractions.Application.AI;

/// <summary>
/// Semantic / vector search - pgvector, Qdrant, AI Search,
/// Pinecone, or an in-process index.
/// </summary>
public interface IVectorSearchService
{
    Task<IReadOnlyList<SearchResult>> SearchAsync(
        string indexName, string query,
        int topK = 10, CancellationToken ct = default);

    Task<IReadOnlyList<SearchResult>> SearchByVectorAsync(
        string indexName, float[] vector,
        int topK = 10, CancellationToken ct = default);

    Task IndexDocumentAsync(
        string indexName, string id,
        object document, float[] vector,
        CancellationToken ct = default);

    Task DeleteDocumentAsync(
        string indexName, string id,
        CancellationToken ct = default);
}
