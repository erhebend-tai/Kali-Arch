using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace VirtualMachineService.API.Grpc.Interceptors;

/// <summary>
/// Server-side — runs a registered IValidator&lt;TRequest&gt; if one
/// exists, mapping failures to InvalidArgument.
/// </summary>
public sealed class ValidationInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest                               request,
        ServerCallContext                      context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        await ValidateAsync(request, context);
        return await continuation(request, context);
    }

    private static async Task ValidateAsync<TRequest>(
        TRequest request, ServerCallContext context)
    {
        var validator = context.GetHttpContext().RequestServices
            .GetService<IValidator<TRequest>>();

        if (validator is null)
            return;

        var result = await validator.ValidateAsync(
            request, context.CancellationToken);

        if (!result.IsValid)
            throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                string.Join("; ", result.Errors.Select(
                    e => $"{e.PropertyName}: {e.ErrorMessage}"))));
    }
}
