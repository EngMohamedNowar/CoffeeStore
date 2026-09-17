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

        // Where Expression
        public Expression<Func<TEntity, bool>> WhereExpression { get; private set; }

        protected void AddWhere(Expression<Func<TEntity, bool>> expression)
        {
            WhereExpression = expression;
        }

        // Order Expression
        public Expression<Func<TEntity, object>> OrderBy { get; private set; }
        public Expression<Func<TEntity, object>> OrderByDescending { get; private set; }

        protected void AddOrderBy(Expression<Func<TEntity, object>> expression)
        {
            OrderBy = expression;
        }
        protected void AddOrderByDescending(Expression<Func<TEntity, object>> expression)
        {
            OrderByDescending = expression;
        }


        // Paging
        public int Skip { get; private set; }
        public int Take { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        protected void ApplyPaging(int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize < 1 ? 10 : pageSize;

            Skip = (pageIndex - 1) * pageSize;
            Take = pageSize;
            IsPagingEnabled = true;
        }
    }
}