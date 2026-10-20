namespace CleanArch.Shared.Abstractions.Application.AI;

/// <summary>Text translation between languages.</summary>
public interface ITranslationService
{
    Task<string> TranslateAsync(
        string text, string targetLanguage,
        string? sourceLanguage = null,
        CancellationToken ct = default);

    Task<string> DetectLanguageAsync(
        string text, CancellationToken ct = default);
}
