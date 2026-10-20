using CleanArch.Application.Interfaces.AI;
using CleanArch.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace CleanArch.Infrastructure.Common.AI.OffDevice;

/// <summary>
/// Off-device LLM via Semantic Kernel — provider plug-ins
/// (Azure OpenAI / OpenAI / Gemini / Ollama connector) are
/// configured on the Kernel at registration time.
/// </summary>
public sealed class SemanticKernelLanguageModelService(
    Kernel kernel,
    IOptions<AIOptions> options)
    : ILanguageModelService
{
    public async Task<string> CompleteAsync(
        string prompt, CancellationToken ct = default)
    {
        using var cts = CancellationTokenSource
            .CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(
            options.Value.OffDevice.TimeoutSeconds));

        var result = await kernel.InvokePromptAsync(
            prompt, cancellationToken: cts.Token);

        return result.GetValue<string>() ?? string.Empty;
    }

    public Task<string> TranslateAsync(
        string text, string targetLanguage,
        CancellationToken ct = default) =>
        CompleteAsync(
            $"Translate the following to {targetLanguage}. " +
            $"Reply with the translation only:\n\n{text}", ct);

    public Task<byte[]> TextToSpeechAsync(
        string text, CancellationToken ct = default) =>
        throw new NotSupportedException(
            "TTS is handled by a speech-specific service.");
}
