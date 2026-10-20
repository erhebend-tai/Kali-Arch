namespace CleanArch.Shared.Abstractions.Application.Observability;

/// <summary>
/// Distributed tracing - wraps ActivitySource / OpenTelemetry
/// so handlers don't take a direct dependency.
/// </summary>
public interface ITracingService
{
    IDisposable StartSpan(
        string operationName,
        IReadOnlyDictionary<string, string>? tags = null);

    void AddTag(string key, string value);
    void RecordException(Exception ex);
}
