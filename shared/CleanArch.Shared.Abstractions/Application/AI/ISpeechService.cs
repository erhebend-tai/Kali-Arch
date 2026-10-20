namespace CleanArch.Shared.Abstractions.Application.AI;

/// <summary>Speech-to-text and text-to-speech.</summary>
public interface ISpeechService
{
    Task<byte[]> TextToSpeechAsync(
        string text, string? voice = null,
        CancellationToken ct = default);

    Task<string> SpeechToTextAsync(
        Stream audio, string? language = null,
        CancellationToken ct = default);
}
