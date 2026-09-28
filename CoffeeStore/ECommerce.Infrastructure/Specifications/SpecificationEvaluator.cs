using ECommerce.Domain.Common;
using ECommerce.Domain.Specification;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Specifications;

public static class SpecificationEvaluator
{
    /// <summary>
    /// Applies includes, filters and ordering from <paramref name="specs"/> to <paramref name="inputQuery"/>.
    /// Paging is only applied when <paramref name="applyPaging"/> is true.
    /// </summary>
    public static IQueryable<TEntity> CreateQuery<TEntity>(
        IQueryable<TEntity> inputQuery,
        ISpecification<TEntity> specs,
        bool applyPaging = true,
        bool applyIncludes = true) where TEntity : BaseEntity
    {
        var query = inputQuery;

        if (applyIncludes)
        {
            foreach (var include in specs.IncludeExpressions)
            {
                query = query.Include(include);
            }
        }

        if (specs.WhereExpression is not null)
        {
            query = query.Where(specs.WhereExpression);
        }

        if (specs.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specs.OrderByDescending);
        }
        else if (specs.OrderBy is not null)
        {
            query = query.OrderBy(specs.OrderBy);
        }

        if (applyPaging && specs.IsPagingEnabled)
        {
            query = query.Skip(specs.Skip).Take(specs.Take);
        }

        return query;
    }

    /// <summary>
    /// Builds the filtered query used for counting: no includes (they would multiply rows through
    /// collection navigations) and no paging, so the result is the true total match count.
    /// </summary>
    public static IQueryable<TEntity> CreateCountQuery<TEntity>(
        IQueryable<TEntity> inputQuery,
        ISpecification<TEntity> specs) where TEntity : BaseEntity
        => CreateQuery(inputQuery, specs, applyPaging: false, applyIncludes: false);
}
