using CleanArch.Application.Interfaces.AI;
using CleanArch.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArch.Infrastructure.Common.AI.Router;

/// <summary>
/// Strategy — picks the best available ILanguageModelService
/// by configured priority, falls back to on-device when all
/// cloud providers fail or the feature flag prefers local.
/// </summary>
public sealed class LanguageModelRouter(
    IEnumerable<NamedLanguageModel> providers,
    IOptions<AIOptions> options,
    ILogger<LanguageModelRouter> logger)
    : ILanguageModelService
{
    public async Task<string> CompleteAsync(
        string prompt, CancellationToken ct = default)
    {
        foreach (var provider in OrderedProviders())
        {
            try
            {
                return await provider.Service
                    .CompleteAsync(prompt, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex,
                    "LLM provider {Provider} failed — trying next.",
                    provider.Name);
            }
        }

        throw new InvalidOperationException(
            "No language model provider succeeded.");
    }

    public Task<string> TranslateAsync(
        string text, string targetLanguage,
        CancellationToken ct = default) =>
        CompleteAsync(
            $"Translate to {targetLanguage}: {text}", ct);

    public Task<byte[]> TextToSpeechAsync(
        string text, CancellationToken ct = default)
    {
        var provider = OrderedProviders().FirstOrDefault();
        if (provider is null)
            throw new InvalidOperationException(
                "No language model provider registered.");

        return provider.Service.TextToSpeechAsync(text, ct);
    }

    private IEnumerable<NamedLanguageModel> OrderedProviders()
    {
        var byName = providers.ToDictionary(p => p.Name, p => p);

        foreach (var name in options.Value.Router.ProviderPriority)
            if (byName.TryGetValue(name, out var provider))
                yield return provider;
    }
}

/// <summary>DI carrier — associates a service with a provider name.</summary>
public sealed record NamedLanguageModel(
    string Name, ILanguageModelService Service);
