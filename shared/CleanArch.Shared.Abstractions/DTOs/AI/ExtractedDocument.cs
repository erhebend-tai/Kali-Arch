namespace CleanArch.Shared.Abstractions.DTOs.AI;

/// <summary>Structured data extracted from a document.</summary>
public sealed record ExtractedDocument(
    string Text,
    IReadOnlyDictionary<string, string> Fields,
    int PageCount,
    double Confidence);
