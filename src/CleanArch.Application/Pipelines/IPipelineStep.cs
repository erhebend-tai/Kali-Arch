namespace CleanArch.Application.Pipelines;

/// <summary>
/// One step in a provisioning pipeline.
/// Steps execute in Order; on failure CompensateAsync
/// runs in reverse order for executed steps.
/// </summary>
public interface IPipelineStep<TContext>
{
    int Order { get; }
    string Name { get; }

    Task<StepResult> ExecuteAsync(
        TContext context, CancellationToken ct = default);

    Task CompensateAsync(
        TContext context, CancellationToken ct = default);
}

public sealed record StepResult(
    bool IsSuccess,
    string? Error = null)
{
    public static StepResult Success() => new(true);
    public static StepResult Failure(string error) => new(false, error);
}
