using ECommerce.Application.Orders.Commands.CancelOrder;
using ECommerce.Application.Orders.Commands.UpdateOrder;
using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Specification;

namespace ECommerce.Tests.Application;

public class OrderCommandHandlerTests
{
    private static Order NewOrder(string email = "ali@example.com")
    {
        var delivery = DeliveryMethod.Create(
            Guid.NewGuid(),
            "Standard",
            15m,
            "2-4 business days",
            isAvailable: true).Value!;

        var address = OrderAddress.Create("Ali", "Hassan", "12 Main St", "Cairo", "Egypt").Value!;

        var item = OrderItem.Create(
            Guid.NewGuid(),
            ProductItemOrdered.Create(Guid.NewGuid(), "Ethiopian Yirgacheffe", 250, 50m).Value!,
            1).Value!;

        return Order.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            email,
            address,
            delivery,
            new[] { item }).Value!;
    }

    private static (CancelOrderCommandHandler Handler, FakeUnitOfWork UnitOfWork) CancelHandler(Order order)
    {
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CancelOrderCommandHandler(
            new FakeReadRepository<Order>([order]),
            unitOfWork,
            new CancelOrderCommandValidator());
        return (handler, unitOfWork);
    }

    private static (UpdateOrderCommandHandler Handler, FakeUnitOfWork UnitOfWork) UpdateHandler(Order order)
    {
        var unitOfWork = new FakeUnitOfWork();
        var handler = new UpdateOrderCommandHandler(
            new FakeReadRepository<Order>([order]),
            unitOfWork,
            new UpdateOrderCommandValidator());
        return (handler, unitOfWork);
    }

    [Fact]
    public async Task CancelOrder_ByOwner_CancelsTheOrder()
    {
        var order = NewOrder();
        var (handler, unitOfWork) = CancelHandler(order);

        var result = await handler.HandleAsync(new CancelOrderCommand(order.Id, "ali@example.com"));

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal(1, unitOfWork.SaveCount);
        Assert.Equal(order.Id, result.Value!.Id);
        Assert.Equal((int)OrderStatus.Cancelled, result.Value.Status);
    }

    [Fact]
    public async Task CancelOrder_MatchesTheEmailCaseInsensitively()
    {
        var order = NewOrder("Ali@Example.com");
        var (handler, _) = CancelHandler(order);

        var result = await handler.HandleAsync(new CancelOrderCommand(order.Id, " ali@example.com "));

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public async Task CancelOrder_ByAnotherUser_IsForbidden()
    {
        var order = NewOrder("ali@example.com");
        var (handler, unitOfWork) = CancelHandler(order);

        var result = await handler.HandleAsync(new CancelOrderCommand(order.Id, "mallory@example.com"));

        Assert.True(result.IsFailure);
        Assert.Equal("Order.Forbidden", Assert.Single(result.Errors).code);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task CancelOrder_WhenTheOrderDoesNotExist_ReturnsNotFound()
    {
        var (handler, _) = CancelHandler(NewOrder());

        var result = await handler.HandleAsync(new CancelOrderCommand(Guid.NewGuid(), "ali@example.com"));

        Assert.True(result.IsFailure);
        Assert.Equal("Order.NotFound", Assert.Single(result.Errors).code);
    }

    [Fact]
    public async Task CancelOrder_WhenAlreadyConfirmed_ReturnsCannotCancel()
    {
        var order = NewOrder();
        order.MarkAsPaid();
        var (handler, unitOfWork) = CancelHandler(order);

        var result = await handler.HandleAsync(new CancelOrderCommand(order.Id, "ali@example.com"));

        Assert.True(result.IsFailure);
        Assert.Equal("Order.CannotCancel", Assert.Single(result.Errors).code);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task CancelOrder_WithAnEmptyId_FailsValidation()
    {
        var (handler, _) = CancelHandler(NewOrder());

        var result = await handler.HandleAsync(new CancelOrderCommand(Guid.Empty, "ali@example.com"));

        Assert.True(result.IsFailure);
        Assert.Equal("Order.Id.Required", Assert.Single(result.Errors).code);
    }

    [Fact]
    public async Task UpdateOrder_WithAValidTransition_UpdatesTheStatus()
    {
        var order = NewOrder();
        var (handler, unitOfWork) = UpdateHandler(order);

        var result = await handler.HandleAsync(new UpdateOrderCommand(order.Id, OrderStatus.Confirmed));

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.Equal(1, unitOfWork.SaveCount);
        Assert.Equal((int)OrderStatus.Confirmed, result.Value!.Status);
    }

    [Fact]
    public async Task UpdateOrder_WithAnIllegalTransition_Fails()
    {
        var order = NewOrder();
        var (handler, unitOfWork) = UpdateHandler(order);

        var result = await handler.HandleAsync(new UpdateOrderCommand(order.Id, OrderStatus.Delivered));

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidStatusTransition", Assert.Single(result.Errors).code);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task UpdateOrder_WhenTheOrderDoesNotExist_ReturnsNotFound()
    {
        var (handler, _) = UpdateHandler(NewOrder());

        var result = await handler.HandleAsync(new UpdateOrderCommand(Guid.NewGuid(), OrderStatus.Confirmed));

        Assert.True(result.IsFailure);
        Assert.Equal("Order.NotFound", Assert.Single(result.Errors).code);
    }

    [Fact]
    public async Task UpdateOrder_WithAnUndefinedStatus_FailsValidation()
    {
        var order = NewOrder();
        var (handler, unitOfWork) = UpdateHandler(order);

        var result = await handler.HandleAsync(new UpdateOrderCommand(order.Id, (OrderStatus)999));

        Assert.True(result.IsFailure);
        Assert.Equal("Order.Status.Invalid", Assert.Single(result.Errors).code);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    private sealed class FakeReadRepository<T>(List<T> storage) : IReadRepository<T>
        where T : BaseEntity
    {
        public Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken ct = default)
        {
            IEnumerable<T> query = storage;

            if (specification.WhereExpression is not null)
                query = query.Where(specification.WhereExpression.Compile());

            return Task.FromResult(query.FirstOrDefault());
        }

        public Task<IReadOnlyList<T>> GetAllAsync(ISpecification<T> specification, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<T>>(storage.ToList());

        public Task<int> CountAsync(ISpecification<T> specification, CancellationToken ct = default)
            => Task.FromResult(storage.Count(specification.WhereExpression!.Compile()));
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken ct)
        {
            SaveCount++;
            return Task.FromResult(1);
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
            => new FakeGenericRepository<TEntity>();

        public IProductRepository ProductRepository()
            => throw new NotSupportedException();
    }

    private sealed class FakeGenericRepository<T> : IGenericRepository<T>
        where T : BaseEntity
    {
        public void Add(T entity)
        {
        }

        public void Update(T entity)
        {
        }

        public void Delete(T entity)
        {
        }

        public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<T>>([]);

        public Task<IReadOnlyList<T>> GetAllAsync(ISpecification<T> specs, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<T>>([]);

        public Task<int> CountAsync(ISpecification<T> specs, CancellationToken ct = default)
            => Task.FromResult(0);
    }
}
