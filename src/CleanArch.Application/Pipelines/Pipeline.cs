using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Pipelines;

public sealed record PipelineResult(
    bool IsSuccess,
    string? Error,
    IReadOnlyList<string> CompletedSteps);

/// <summary>
/// Executes steps in Order. On failure, compensates executed
/// steps in reverse (saga-style rollback).
/// </summary>
public abstract class Pipeline<TContext>(ILogger logger)
    where TContext : PipelineContext
{
    protected abstract IReadOnlyList<IPipelineStep<TContext>> Steps { get; }

    public async Task<PipelineResult> ExecuteAsync(
        TContext context, CancellationToken ct = default)
    {
        var executed = new List<IPipelineStep<TContext>>();

        foreach (var step in Steps.OrderBy(s => s.Order))
        {
            logger.LogInformation("Executing pipeline step: {Step}", step.Name);

            var result = await step.ExecuteAsync(context, ct);

            if (!result.IsSuccess)
            {
                context.ErrorMessage =
                    $"Step '{step.Name}' failed: {result.Error}";
                await CompensateAsync(executed, context, ct);
                return new PipelineResult(
                    false, context.ErrorMessage,
                    context.CompletedSteps);
            }

            executed.Add(step);
            context.CompletedSteps.Add(step.Name);
        }

        return new PipelineResult(true, null, context.CompletedSteps);
    }

    private async Task CompensateAsync(
        List<IPipelineStep<TContext>> executed,
        TContext context,
        CancellationToken ct)
    {
        for (var i = executed.Count - 1; i >= 0; i--)
        {
            var step = executed[i];
            logger.LogInformation("Compensating step: {Step}", step.Name);
            try { await step.CompensateAsync(context, ct); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Compensation failed for {Step}", step.Name);
            }
        }
    }
}
