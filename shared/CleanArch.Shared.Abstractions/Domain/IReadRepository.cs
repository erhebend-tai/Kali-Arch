using CleanArch.Shared.Domain.Primitives;

namespace CleanArch.Shared.Abstractions.Domain;

/// <summary>
/// Query-only repository - no mutation paths. Implemented by
/// read-side persistence (Dapper, read replicas, projections).
/// </summary>
public interface IReadRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> FindAsync(
        ISpecification<T> specification, CancellationToken ct = default);
    Task<int> CountAsync(
        ISpecification<T> specification, CancellationToken ct = default);
    Task<bool> AnyAsync(
        ISpecification<T> specification, CancellationToken ct = default);
}
