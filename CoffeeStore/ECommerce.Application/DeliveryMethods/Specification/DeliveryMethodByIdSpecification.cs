using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.DeliveryMethods.Specification;

public class DeliveryMethodByIdSpecification : BaseSpecification<DeliveryMethod>
{
    public DeliveryMethodByIdSpecification(Guid id)
        : base(dm => dm.Id == id)
    {
        AddSelect(dm => new { dm.Id, dm.Name, dm.Description, dm.Price, dm.EstimatedDeliveryTime, dm.IsAvailable, dm.DisplayOrder });
    }
}
