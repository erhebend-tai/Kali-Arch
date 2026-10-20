namespace CleanArch.Shared.Abstractions.Domain;

/// <summary>
/// Shared state passed through every step - steps read from it,
/// mutate it, and pass it to the next step.
/// </summary>
public class PipelineContext
{
    public Dictionary<string, object> Data { get; } = [];
    public List<string> CompletedSteps { get; } = [];
    public string? ErrorMessage { get; set; }

    public T? Get<T>(string key) =>
        Data.TryGetValue(key, out var value) ? (T)value : default;

    public void Set<T>(string key, T value) where T : notnull =>
        Data[key] = value;
}
