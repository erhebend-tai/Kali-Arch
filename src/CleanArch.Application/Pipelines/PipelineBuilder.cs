using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Pipelines;

/// <summary>
/// Fluent builder for composing a step list at runtime.
/// </summary>
public sealed class PipelineBuilder<TContext>(
    IServiceProvider serviceProvider,
    ILoggerFactory loggerFactory)
    where TContext : PipelineContext
{
    private readonly List<IPipelineStep<TContext>> _steps = [];

    public PipelineBuilder<TContext> AddStep<TStep>()
        where TStep : IPipelineStep<TContext>
    {
        _steps.Add(serviceProvider.GetRequiredService<TStep>());
        return this;
    }

    public PipelineBuilder<TContext> AddStep(IPipelineStep<TContext> step)
    {
        _steps.Add(step);
        return this;
    }

    public AdHocPipeline Build() => new(_steps, loggerFactory);

    public sealed class AdHocPipeline(
        IReadOnlyList<IPipelineStep<TContext>> steps,
        ILoggerFactory loggerFactory)
        : Pipeline<TContext>(
            loggerFactory.CreateLogger<AdHocPipeline>())
    {
        protected override IReadOnlyList<IPipelineStep<TContext>> Steps
            => steps;
    }
}
