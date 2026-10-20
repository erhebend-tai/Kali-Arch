namespace CleanArch.Shared.Abstractions.Application.Observability;

/// <summary>
/// Structured logging abstraction - for components that should
/// not take a direct ILogger dependency (domain services, plugins).
/// </summary>
public interface ILoggingService
{
    void LogInformation(
        string message,
        IReadOnlyDictionary<string, object?>? properties = null);

    void LogWarning(
        string message,
        IReadOnlyDictionary<string, object?>? properties = null);

    void LogError(
        string message, Exception? exception = null,
        IReadOnlyDictionary<string, object?>? properties = null);
}
