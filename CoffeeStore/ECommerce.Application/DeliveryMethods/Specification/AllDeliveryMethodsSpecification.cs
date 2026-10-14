using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.DeliveryMethods.Specification;

public class AllDeliveryMethodsSpecification : BaseSpecification<DeliveryMethod>
{
    public AllDeliveryMethodsSpecification()
    {
        AddOrderBy(dm => dm.DisplayOrder);
    }
}
