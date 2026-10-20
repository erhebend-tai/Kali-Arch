namespace CleanArch.Application.Options;

public sealed class AIOptions
{
    public const string SectionName = "AI";

    public OnDeviceOptions OnDevice { get; init; } = new();
    public OffDeviceOptions OffDevice { get; init; } = new();
    public RouterOptions Router { get; init; } = new();
}

public sealed class OnDeviceOptions
{
    public bool Enabled { get; init; }
    public string ModelsDirectory { get; init; } = "models/";
    public int MaxContextSize { get; init; } = 4096;
    public int GpuLayers { get; init; }
    public int Threads { get; init; } = 4;

    public Dictionary<string, string> ModelPaths { get; init; } = new()
    {
        ["phi3-mini"]  = "models/phi3-mini/",
        ["llama32-3b"] =
            "models/llama32-3b/Llama-3.2-3B-Instruct-Q4_K_M.gguf",
        ["bge-small"]  = "models/bge-small/onnx/"
    };
}

public sealed class OffDeviceOptions
{
    public string PrimaryProvider { get; init; } = "AzureOpenAI";
    public int MaxTokens { get; init; } = 2048;
    public double Temperature { get; init; } = 0.7;
    public int TimeoutSeconds { get; init; } = 30;
}

public sealed class RouterOptions
{
    public IReadOnlyList<string> ProviderPriority { get; init; } =
        ["AzureOpenAI", "Anthropic", "GoogleGemini", "LlamaSharp"];

    public bool FallbackToOnDevice { get; init; } = true;
}
