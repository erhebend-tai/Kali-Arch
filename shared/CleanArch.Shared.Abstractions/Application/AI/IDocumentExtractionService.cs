using CleanArch.Shared.Abstractions.DTOs.AI;

namespace CleanArch.Shared.Abstractions.Application.AI;

/// <summary>
/// Document intelligence - structured extraction from
/// PDFs, invoices, forms.
/// </summary>
public interface IDocumentExtractionService
{
    Task<ExtractedDocument> ExtractAsync(
        Stream document, string contentType,
        CancellationToken ct = default);
}
