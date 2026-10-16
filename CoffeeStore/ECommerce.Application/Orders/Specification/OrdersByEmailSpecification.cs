using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Orders.Specification;

public class OrdersByEmailSpecification : BaseSpecification<Order>
{
    public OrdersByEmailSpecification(string email)
        : base(o => o.UserEmail == email)
    {
        AddInclude(o => o.DeliveryMethod);
        AddInclude(o => o.Items);
        AddOrderByDescending(o => o.CreatedAt);
    }
}
