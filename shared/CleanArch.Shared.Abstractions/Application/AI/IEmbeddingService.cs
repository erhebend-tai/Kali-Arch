namespace CleanArch.Shared.Abstractions.Application.AI;

/// <summary>Embedding vector generation - on- or off-device.</summary>
public interface IEmbeddingService
{
    /// <summary>Vector dimensionality produced by this model.</summary>
    int Dimensions { get; }

    Task<float[]> EmbedAsync(string text, CancellationToken ct = default);

    Task<IReadOnlyList<float[]>> EmbedBatchAsync(
        IEnumerable<string> texts, CancellationToken ct = default);
}
