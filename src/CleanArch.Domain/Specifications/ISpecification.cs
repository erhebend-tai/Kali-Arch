using System.Linq.Expressions;

namespace CleanArch.Domain.Specifications;

/// <summary>
/// Composable business rule — translates to an IQueryable
/// by SpecificationEvaluator in Infrastructure.Common/Persistence.
/// </summary>
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int? Take { get; }
    int? Skip { get; }
    bool IsPagingEnabled { get; }

    bool IsSatisfiedBy(T entity);

    ISpecification<T> And(ISpecification<T> other);
    ISpecification<T> Or(ISpecification<T> other);
    ISpecification<T> Not();
}
