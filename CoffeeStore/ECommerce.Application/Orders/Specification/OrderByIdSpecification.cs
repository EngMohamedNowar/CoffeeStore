using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Orders.Specification;

public class OrderByIdSpecification : BaseSpecification<Order>
{
    public OrderByIdSpecification(Guid id)
        : base(o => o.Id == id)
    {
        AddInclude(o => o.DeliveryMethod);
        AddInclude(o => o.Items);
    }
}
