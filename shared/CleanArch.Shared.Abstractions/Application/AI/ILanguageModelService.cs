namespace CleanArch.Shared.Abstractions.Application.AI;

/// <summary>
/// Cloud- and runtime-agnostic LLM abstraction -
/// implemented by ONNX/LLamaSharp (on-device) and
/// SemanticKernel/OpenAI/Anthropic/Gemini/Ollama (off-device).
/// </summary>
public interface ILanguageModelService
{
    Task<string> CompleteAsync(
        string prompt, CancellationToken ct = default);

    Task<string> CompleteWithSystemAsync(
        string systemPrompt, string userPrompt,
        CancellationToken ct = default);

    Task<string> TranslateAsync(
        string text, string targetLanguage,
        CancellationToken ct = default);

    Task<byte[]> TextToSpeechAsync(
        string text, CancellationToken ct = default);

    IAsyncEnumerable<string> StreamAsync(
        string prompt, CancellationToken ct = default);
}
