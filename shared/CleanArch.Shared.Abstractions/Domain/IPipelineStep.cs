namespace CleanArch.Shared.Abstractions.Domain;

/// <summary>
/// One step in a provisioning pipeline.
/// Steps execute in Order; on failure CompensateAsync
/// runs in reverse order for executed steps.
/// </summary>
public interface IPipelineStep<in TContext> where TContext : PipelineContext
{
    int Order { get; }
    string Name { get; }

    Task<StepResult> ExecuteAsync(
        TContext context, CancellationToken ct = default);

    Task CompensateAsync(
        TContext context, CancellationToken ct = default);
}
