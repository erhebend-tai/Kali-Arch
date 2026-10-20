namespace CleanArch.Application.Interfaces.AI;

/// <summary>
/// Cloud- and runtime-agnostic LLM abstraction —
/// implemented by ONNX/LLamaSharp (on-device) and
/// SemanticKernel/OpenAI/Anthropic/Gemini/Ollama (off-device).
/// </summary>
public interface ILanguageModelService
{
    Task<string> CompleteAsync(
        string prompt, CancellationToken ct = default);

    Task<string> TranslateAsync(
        string text, string targetLanguage,
        CancellationToken ct = default);

    Task<byte[]> TextToSpeechAsync(
        string text, CancellationToken ct = default);
}
