namespace CleanArch.Shared.Abstractions.DTOs.AI;

/// <summary>Output of image analysis.</summary>
public sealed record ImageAnalysisResult(
    string? Caption,
    IReadOnlyList<string> Tags,
    double Confidence,
    IReadOnlyDictionary<string, string>? Metadata = null);
