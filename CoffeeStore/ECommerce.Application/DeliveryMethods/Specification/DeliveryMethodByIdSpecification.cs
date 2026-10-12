using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.DeliveryMethods.Specification;

public class DeliveryMethodByIdSpecification : BaseSpecification<DeliveryMethod>
{
    public DeliveryMethodByIdSpecification(Guid id)
        : base(dm => dm.Id == id)
    {
    }
}
