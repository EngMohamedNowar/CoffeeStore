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
        public Expression<Func<TEntity, object>> OrderByNameDsc { get; private set; }

        public Expression<Func<TEntity, object>> OrderByNameAsc { get; private set; }

        public Expression<Func<TEntity, object>> OrderByPriceAsc { get; private set; }

        public Expression<Func<TEntity, object>> OrderByPriceDsc { get; private set; }


        protected void AddOrderByAscendingName(Expression<Func<TEntity, object>> expression)
        {
            OrderByNameAsc = expression;
        }

        protected void AddOrderByDescendingName(Expression<Func<TEntity, object>> expression)
        {
            OrderByNameDsc = expression;
        }

        protected void AddOrderByPriceAscending(Expression<Func<TEntity, object>> expression)
        {
            OrderByPriceAsc = expression;
        }

        protected void AddOrderByPriceDscending(Expression<Func<TEntity, object>> expression)
        {
            OrderByPriceDsc = expression;
        }


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