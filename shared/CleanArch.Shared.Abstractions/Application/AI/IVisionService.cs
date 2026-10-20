using CleanArch.Shared.Abstractions.DTOs.AI;

namespace CleanArch.Shared.Abstractions.Application.AI;

/// <summary>Image analysis - captioning, tagging, OCR.</summary>
public interface IVisionService
{
    Task<ImageAnalysisResult> AnalyzeAsync(
        Stream image, CancellationToken ct = default);

    Task<string> ExtractTextAsync(
        Stream image, CancellationToken ct = default);
}
