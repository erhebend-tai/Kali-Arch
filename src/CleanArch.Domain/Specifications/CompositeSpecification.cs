using System.Linq.Expressions;

namespace CleanArch.Domain.Specifications;

internal static class ExpressionComposer
{
    public static Expression<Func<T, bool>> AndAlso<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right) =>
        Combine(left, right, Expression.AndAlso);

    public static Expression<Func<T, bool>> OrElse<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right) =>
        Combine(left, right, Expression.OrElse);

    public static Expression<Func<T, bool>> Negate<T>(
        Expression<Func<T, bool>> inner)
    {
        var param = Expression.Parameter(typeof(T), "x");
        return Expression.Lambda<Func<T, bool>>(
            Expression.Not(Expression.Invoke(inner, param)), param);
    }

    private static Expression<Func<T, bool>> Combine<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, BinaryExpression> merge)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var body = merge(
            Expression.Invoke(left, param),
            Expression.Invoke(right, param));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}

public sealed class AndSpecification<T>(
    ISpecification<T> left, ISpecification<T> right)
    : BaseSpecification<T>(
        ExpressionComposer.AndAlso(left.Criteria, right.Criteria))
{
}

public sealed class OrSpecification<T>(
    ISpecification<T> left, ISpecification<T> right)
    : BaseSpecification<T>(
        ExpressionComposer.OrElse(left.Criteria, right.Criteria))
{
}

public sealed class NotSpecification<T>(ISpecification<T> inner)
    : BaseSpecification<T>(ExpressionComposer.Negate(inner.Criteria))
{
}
