using CleanArch.Application.Interfaces;
using MediatR;

namespace CleanArch.Application.Behaviors;

/// <summary>
/// Opt-in authorization — requests implementing IRequireAuthorization
/// declare the roles they need; the behavior enforces them.
/// </summary>
public interface IRequireAuthorization
{
    IReadOnlyList<string> RequiredRoles { get; }
}

public sealed class AuthorizationBehavior<TRequest, TResponse>(
    ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequireAuthorization
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (currentUserService.UserId is null)
            throw new UnauthorizedAccessException(
                "Authentication required.");

        foreach (var role in request.RequiredRoles)
            if (!currentUserService.IsInRole(role))
                throw new UnauthorizedAccessException(
                    $"Role '{role}' required.");

        return await next();
    }
}
