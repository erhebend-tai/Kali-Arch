using CleanArch.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Common.Persistence;

/// <summary>
/// Translates an ISpecification into an IQueryable —
/// criteria, includes, ordering, paging.
/// </summary>
public static class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(
        IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery.Where(specification.Criteria);

        query = specification.Includes.Aggregate(
            query, (current, include) => current.Include(include));

        if (specification.OrderBy is not null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending is not null)
            query = query.OrderByDescending(specification.OrderByDescending);

        if (specification.IsPagingEnabled)
            query = query
                .Skip(specification.Skip ?? 0)
                .Take(specification.Take ?? int.MaxValue);

        return query;
    }
}
