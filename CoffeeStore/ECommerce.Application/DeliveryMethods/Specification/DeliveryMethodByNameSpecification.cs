using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.DeliveryMethods.Specification;

public class DeliveryMethodByNameSpecification : BaseSpecification<DeliveryMethod>
{
    public DeliveryMethodByNameSpecification(string name)
        : base(dm => dm.Name.ToLower() == name.ToLower())
    {
    }
}
