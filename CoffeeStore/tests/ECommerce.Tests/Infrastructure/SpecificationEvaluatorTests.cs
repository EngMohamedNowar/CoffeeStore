using ECommerce.Domain.Common;
using ECommerce.Domain.Entities.Categories;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Specification;
using ECommerce.Infrastructure.Specifications;

namespace ECommerce.Tests.Infrastructure;

public class SpecificationEvaluatorTests
{
    private sealed class TestSpecification : ISpecification<TestEntity>
    {
        public ICollection<System.Linq.Expressions.Expression<Func<TestEntity, object>>> IncludeExpressions { get; } = [];

        public System.Linq.Expressions.Expression<Func<TestEntity, bool>>? WhereExpression { get; init; }
        public System.Linq.Expressions.Expression<Func<TestEntity, object>>? OrderBy { get; init; }
        public System.Linq.Expressions.Expression<Func<TestEntity, object>>? OrderByDescending { get; init; }
        public int Take { get; init; }
        public int Skip { get; init; }
        public bool IsPagingEnabled { get; init; }
    }

    private sealed class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Rank { get; set; }
    }

    private static IQueryable<TestEntity> Seed() => Enumerable.Range(1, 20)
        .Select(i => new TestEntity { Name = $"item-{i:D2}", Rank = i })
        .AsQueryable();

    [Fact]
    public void CreateQuery_AppliesWhereClause()
    {
        var specs = new TestSpecification { WhereExpression = e => e.Rank <= 5 };

        var result = SpecificationEvaluator.CreateQuery(Seed(), specs).ToList();

        Assert.Equal(5, result.Count);
        Assert.All(result, e => Assert.True(e.Rank <= 5));
    }

    [Fact]
    public void CreateQuery_AppliesOrdering()
    {
        var specs = new TestSpecification { OrderBy = e => e.Rank };

        var result = SpecificationEvaluator.CreateQuery(Seed(), specs).ToList();

        Assert.Equal(result.Select(e => e.Rank).Order(), result.Select(e => e.Rank));
    }

    [Fact]
    public void CreateQuery_AppliesDescendingOrdering()
    {
        var specs = new TestSpecification { OrderByDescending = e => e.Rank };

        var result = SpecificationEvaluator.CreateQuery(Seed(), specs).ToList();

        Assert.Equal(result.Select(e => e.Rank).OrderDescending(), result.Select(e => e.Rank));
    }

    [Fact]
    public void CreateQuery_AppliesPagingWhenEnabled()
    {
        var specs = new TestSpecification
        {
            OrderBy = e => e.Rank,
            Skip = 10,
            Take = 3,
            IsPagingEnabled = true
        };

        var result = SpecificationEvaluator.CreateQuery(Seed(), specs).ToList();

        Assert.Equal([11, 12, 13], result.Select(e => e.Rank));
    }

    [Fact]
    public void CreateQuery_IgnoresPagingWhenDisabled()
    {
        var specs = new TestSpecification
        {
            Skip = 10,
            Take = 3,
            IsPagingEnabled = false
        };

        var result = SpecificationEvaluator.CreateQuery(Seed(), specs).ToList();

        Assert.Equal(20, result.Count);
    }

    [Fact]
    public void CreateCountQuery_ReturnsTheFullMatchCountDespitePagingOnTheSpecification()
    {
        // Regression: CountAsync used to evaluate the paged query, so a spec carrying
        // paging silently reported min(total, pageSize) instead of the real total.
        var specs = new TestSpecification
        {
            WhereExpression = e => e.Rank <= 15,
            Skip = 0,
            Take = 5,
            IsPagingEnabled = true
        };

        var count = SpecificationEvaluator.CreateCountQuery(Seed(), specs).Count();

        Assert.Equal(15, count);
    }

    [Fact]
    public void CreateCountQuery_AppliesTheFilter()
    {
        var specs = new TestSpecification { WhereExpression = e => e.Rank > 10 };

        var count = SpecificationEvaluator.CreateCountQuery(Seed(), specs).Count();

        Assert.Equal(10, count);
    }

    [Fact]
    public void CreateCountQuery_IsNotAffectedByApplyPagingFlagOnTheSpec()
    {
        var specs = new TestSpecification { IsPagingEnabled = true, Take = 2, Skip = 0 };

        var count = SpecificationEvaluator.CreateCountQuery(Seed(), specs).Count();

        Assert.Equal(20, count);
    }
}
