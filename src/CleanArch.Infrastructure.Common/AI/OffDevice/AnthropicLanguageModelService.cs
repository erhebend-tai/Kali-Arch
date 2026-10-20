using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CleanArch.Application.Interfaces.AI;
using CleanArch.Application.Options;
using Microsoft.Extensions.Options;

namespace CleanArch.Infrastructure.Common.AI.OffDevice;

/// <summary>Anthropic Messages API — plain HTTP.</summary>
public sealed class AnthropicLanguageModelService(
    HttpClient http,
    IOptions<AIOptions> options)
    : ILanguageModelService
{
    public async Task<string> CompleteAsync(
        string prompt, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(
            "v1/messages",
            new AnthropicRequest(
                Model: "claude-haiku-4-5-20251001",
                MaxTokens: options.Value.OffDevice.MaxTokens,
                Messages:
                [
                    new AnthropicMessage("user", prompt)
                ]),
            ct);

        response.EnsureSuccessStatusCode();
        var result = await response.Content
            .ReadFromJsonAsync<AnthropicResponse>(ct);

        return result?.Content.FirstOrDefault()?.Text ?? string.Empty;
    }

    public Task<string> TranslateAsync(
        string text, string targetLanguage,
        CancellationToken ct = default) =>
        CompleteAsync(
            $"Translate to {targetLanguage}: {text}", ct);

    public Task<byte[]> TextToSpeechAsync(
        string text, CancellationToken ct = default) =>
        throw new NotSupportedException(
            "Anthropic does not expose TTS.");

    private sealed record AnthropicRequest(
        string Model,
        [property: JsonPropertyName("max_tokens")] int MaxTokens,
        List<AnthropicMessage> Messages);

    private sealed record AnthropicMessage(string Role, string Content);

    private sealed record AnthropicResponse(
        List<AnthropicContent> Content);

    private sealed record AnthropicContent(string Text);
}
