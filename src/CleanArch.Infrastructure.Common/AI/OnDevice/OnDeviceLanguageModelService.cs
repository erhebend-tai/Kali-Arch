using CleanArch.Application.Interfaces.AI;
using CleanArch.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArch.Infrastructure.Common.AI.OnDevice;

/// <summary>
/// Native inference backend — implemented by a GGUF runtime
/// (LLamaSharp / onnxruntime-genai) without leaking the
/// native types into the service.
/// </summary>
public interface IGgufModelBackend
{
    Task<string> InferAsync(
        string modelPath, string prompt,
        int maxTokens, double temperature,
        CancellationToken ct);
}

/// <summary>
/// On-device LLM — zero network, model files bundled or
/// downloaded to ModelsDirectory. The heavy runtime
/// (LLamaSharp.Backend.Cpu / GenAI) implements
/// IGgufModelBackend behind this boundary.
/// </summary>
public sealed class OnDeviceLanguageModelService(
    IGgufModelBackend backend,
    IOptions<AIOptions> options,
    ILogger<OnDeviceLanguageModelService> logger)
    : ILanguageModelService
{
    public Task<string> CompleteAsync(
        string prompt, CancellationToken ct = default)
    {
        var opts = options.Value;
        var modelPath = opts.OnDevice.ModelPaths
            .GetValueOrDefault("llama32-3b",
                "models/llama32-3b/model.gguf");

        logger.LogInformation(
            "On-device inference via {Model} ({Tokens} max)",
            modelPath, opts.OffDevice.MaxTokens);

        return backend.InferAsync(
            modelPath, prompt,
            opts.OffDevice.MaxTokens,
            opts.OffDevice.Temperature, ct);
    }

    public Task<string> TranslateAsync(
        string text, string targetLanguage,
        CancellationToken ct = default) =>
        CompleteAsync(
            $"Translate to {targetLanguage}: {text}", ct);

    public Task<byte[]> TextToSpeechAsync(
        string text, CancellationToken ct = default) =>
        throw new NotSupportedException(
            "On-device TTS requires a separate speech model.");
}
