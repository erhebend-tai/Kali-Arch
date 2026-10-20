using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Chains;

/// <summary>
/// Chain of Responsibility — handlers linked at runtime, each
/// deciding whether to handle, pass on, or short-circuit.
/// </summary>
public abstract class RequestChainHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private RequestChainHandler<TRequest, TResponse>? _next;

    public RequestChainHandler<TRequest, TResponse> SetNext(
        RequestChainHandler<TRequest, TResponse> next)
    {
        _next = next;
        return this;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request, CancellationToken ct = default)
    {
        if (await CanHandleAsync(request, ct))
            return await HandleInternalAsync(request, ct);

        if (_next is not null)
            return await _next.HandleAsync(request, ct);

        return await FallbackAsync(request, ct);
    }

    protected abstract Task<bool> CanHandleAsync(
        TRequest request, CancellationToken ct);

    protected abstract Task<TResponse> HandleInternalAsync(
        TRequest request, CancellationToken ct);

    protected virtual Task<TResponse> FallbackAsync(
        TRequest request, CancellationToken ct) =>
        throw new InvalidOperationException(
            $"No handler in chain could process {typeof(TRequest).Name}.");
}
