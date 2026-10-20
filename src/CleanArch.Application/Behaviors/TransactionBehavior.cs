using CleanArch.Application.Common;
using CleanArch.Application.Interfaces;
using MediatR;

namespace CleanArch.Application.Behaviors;

/// <summary>
/// Wraps commands in a Unit of Work transaction.
/// Queries and notifications pass through untransacted.
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var response = await next();
            await unitOfWork.CommitAsync(ct);
            return response;
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
