using ECommerce.Domain.Common;
using System.Linq.Expressions;

namespace ECommerce.Domain.Specification;

public interface ISpecification<TEntity> where TEntity : BaseEntity
{
    ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; }

    Expression<Func<TEntity, bool>>? WhereExpression { get; }

    Expression<Func<TEntity, object>>? OrderBy { get; }

    Expression<Func<TEntity, object>>? OrderByDescending { get; }

    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}
