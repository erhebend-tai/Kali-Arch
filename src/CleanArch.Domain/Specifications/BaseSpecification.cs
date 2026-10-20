using System.Linq.Expressions;

namespace CleanArch.Domain.Specifications;

public abstract class BaseSpecification<T>(Expression<Func<T, bool>> criteria)
    : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; } = criteria;
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int? Take { get; private set; }
    public int? Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public bool IsSatisfiedBy(T entity) => Criteria.Compile()(entity);

    protected void AddInclude(Expression<Func<T, object>> include) =>
        Includes.Add(include);

    protected void ApplyOrderBy(Expression<Func<T, object>> orderBy) =>
        OrderBy = orderBy;

    protected void ApplyOrderByDescending(
        Expression<Func<T, object>> orderByDescending) =>
        OrderByDescending = orderByDescending;

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    public ISpecification<T> And(ISpecification<T> other) =>
        new AndSpecification<T>(this, other);

    public ISpecification<T> Or(ISpecification<T> other) =>
        new OrSpecification<T>(this, other);

    public ISpecification<T> Not() => new NotSpecification<T>(this);
}
