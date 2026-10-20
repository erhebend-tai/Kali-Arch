namespace CleanArch.Shared.Abstractions.DTOs.AI;

/// <summary>One hit from a vector / semantic search.</summary>
public sealed record SearchResult(
    string Id,
    string Content,
    double Score,
    IReadOnlyDictionary<string, object>? Metadata = null);
