using ECommerce.Domain.Common;
using ECommerce.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ECommerce.Application.Specifications
{
    public class BaseSpecification<TEntity> : ISpecification<TEntity> where TEntity : BaseEntity
    {
        protected BaseSpecification()
        {
        }

        protected BaseSpecification(Expression<Func<TEntity, bool>> whereExpression)
        {
            WhereExpression = whereExpression;
        }

        // Include Condition
        public ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = [];

        protected void AddInclude(Expression<Func<TEntity, object>> expression)
        {
            IncludeExpressions.Add(expression);
        }

        // Where Condition
        public Expression<Func<TEntity, bool>> WhereExpression { get; private set; }

        protected void AddWhere(Expression<Func<TEntity, bool>> expression)
        {
            WhereExpression = expression;
        }

        //// Order By Condition
        //public Expression<Func<TEntity, object>> OrderByExpression { get; private set; }

        //protected void AddOrderBy(Expression<Func<TEntity, object>> expression)
        //{
        //    OrderByExpression = expression;
        //}

        //// Order By Descending Condition
        //public Expression<Func<TEntity, object>> OrderByDescendingExpression { get; private set; }

        //protected void AddOrderByDescending(Expression<Func<TEntity, object>> expression)
        //{
        //    OrderByDescendingExpression = expression;
        //}

        //// Paging
        //public int Skip { get; private set; }
        //public int Take { get; private set; }
        //public bool IsPagingEnabled { get; private set; }

        //protected void ApplyPaging(int skip, int take)
        //{
        //    Skip = skip;
        //    Take = take;
        //    IsPagingEnabled = true;
        //}
    }
}