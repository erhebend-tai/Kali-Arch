namespace CleanArch.Application.Interfaces;

/// <summary>
/// Coordinates repository operations with the transaction —
/// all-or-nothing commits, domain events dispatched on commit.
/// Implemented by Infrastructure.Common/Persistence/UnitOfWork.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
