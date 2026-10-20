using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CleanArch.Application.Interfaces.AI;
using CleanArch.Application.Options;
using Microsoft.Extensions.Options;

namespace CleanArch.Infrastructure.Common.AI.OffDevice;

/// <summary>
/// Ollama — runs self-hosted models on a server or dev box.
/// Plain HTTP, no SDK dependency.
/// </summary>
public sealed class OllamaLanguageModelService(
    HttpClient http,
    IOptions<AIOptions> options)
    : ILanguageModelService
{
    public async Task<string> CompleteAsync(
        string prompt, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(
            "api/generate",
            new OllamaRequest(
                Model: "llama3.2",
                Prompt: prompt,
                Stream: false,
                Options: new(
                    options.Value.OffDevice.MaxTokens,
                    options.Value.OffDevice.Temperature)),
            ct);

        response.EnsureSuccessStatusCode();
        var result = await response.Content
            .ReadFromJsonAsync<OllamaResponse>(ct);

        return result?.Response ?? string.Empty;
    }

    public Task<string> TranslateAsync(
        string text, string targetLanguage,
        CancellationToken ct = default) =>
        CompleteAsync(
            $"Translate to {targetLanguage}: {text}", ct);

    public Task<byte[]> TextToSpeechAsync(
        string text, CancellationToken ct = default) =>
        throw new NotSupportedException(
            "Ollama does not expose TTS.");

    private sealed record OllamaRequest(
        string Model, string Prompt, bool Stream,
        OllamaOptions Options);

    private sealed record OllamaOptions(
        [property: JsonPropertyName("num_predict")] int MaxTokens,
        double Temperature);

    private sealed record OllamaResponse(string Response);
}
