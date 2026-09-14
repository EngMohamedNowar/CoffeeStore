using ECommerce.Domain.Common;
using ECommerce.Domain.Specification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Specifications
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity>(IQueryable<TEntity> inputQuery,ISpecification<TEntity> specs) where TEntity : BaseEntity
        {
            var query = inputQuery;

            if (specs.IncludeExpressions.Any())
            {
                foreach (var expression in specs.IncludeExpressions)
                {
                    query = query.Include(expression);
                }
            }

            if (specs.WhereExpression is not null)
                query = query.Where(specs.WhereExpression);

            return query;

        }
    }
}
