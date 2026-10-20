using CleanArch.Application.Interfaces.AI;
using CleanArch.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace CleanArch.Infrastructure.Common.AI.OnDevice;

/// <summary>
/// On-device embeddings via ONNX Runtime — e.g. bge-small-en.
/// Swap in Microsoft.ML.OnnxRuntimeGenAI for GenAI-family models.
/// </summary>
public sealed class OnnxEmbeddingService(
    IOptions<AIOptions> options,
    ILogger<OnnxEmbeddingService> logger)
    : IEmbeddingService, IDisposable
{
    private InferenceSession? _session;
    private readonly object _lock = new();

    public Task<float[]> EmbedAsync(
        string text, CancellationToken ct = default)
    {
        var session = GetSession();

        // Tokenization is model-specific — a real deployment plugs
        // in the tokenizer matching the .onnx file. Shape here is a
        // single pooled vector read from the model output.
        var inputIds = Tokenize(text);
        var seqLen = inputIds.Length;

        var inputTensor = new DenseTensor<long>(
            inputIds, [1, seqLen]);
        var maskTensor = new DenseTensor<long>(
            Enumerable.Repeat(1L, seqLen).ToArray(), [1, seqLen]);

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input_ids", inputTensor),
            NamedOnnxValue.CreateFromTensor(
                "attention_mask", maskTensor)
        };

        using var results = session.Run(inputs);
        var output = results.First().AsTensor<float>();
        var embedding = output.ToArray();

        return Task.FromResult(embedding);
    }

    private InferenceSession GetSession()
    {
        if (_session is not null)
            return _session;

        lock (_lock)
        {
            _session ??= CreateSession();
            return _session;
        }
    }

    private InferenceSession CreateSession()
    {
        var opts = options.Value.OnDevice;
        var modelDir = opts.ModelPaths
            .GetValueOrDefault("bge-small", "models/bge-small/onnx/");
        var modelPath = Path.Combine(modelDir, "model.onnx");

        logger.LogInformation(
            "Loading ONNX embedding model from {Path}", modelPath);

        var sessionOptions = new SessionOptions
        {
            IntraOpNumThreads = opts.Threads
        };

        return new InferenceSession(modelPath, sessionOptions);
    }

    private static long[] Tokenize(string text)
    {
        // Placeholder tokenization — replace with the model's real
        // tokenizer (e.g. Microsoft.ML.Tokenizers + vocab file).
        var ids = new List<long> { 101 }; // [CLS]
        ids.AddRange(text.Take(510)
            .Select(c => (long)(c % 30000) + 999));
        ids.Add(102); // [SEP]
        return ids.ToArray();
    }

    public void Dispose() => _session?.Dispose();
}
