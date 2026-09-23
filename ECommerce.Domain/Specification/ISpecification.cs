using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ECommerce.Domain.Specification
{
    public interface ISpecification<TEntity> where TEntity : BaseEntity
    {
        // Include Expression
        public ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; }

        // Where Expression
        Expression<Func<TEntity, bool>> WhereExpression { get; }

        // OrderBy Expression
        Expression<Func<TEntity, object>> OrderBy { get; }
        Expression<Func<TEntity, object>> OrderByDescending { get; }

        // Pagination
        int Take { get;}
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
