namespace CleanArch.Application.Interfaces.AI;

/// <summary>Embedding vector generation — on- or off-device.</summary>
public interface IEmbeddingService
{
    Task<float[]> EmbedAsync(string text, CancellationToken ct = default);
}
