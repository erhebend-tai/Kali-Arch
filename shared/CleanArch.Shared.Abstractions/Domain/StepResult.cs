namespace CleanArch.Shared.Abstractions.Domain;

/// <summary>Outcome of a single pipeline step.</summary>
public sealed record StepResult(
    bool IsSuccess,
    string? Error = null)
{
    public static StepResult Success() => new(true);
    public static StepResult Failure(string error) => new(false, error);
}
