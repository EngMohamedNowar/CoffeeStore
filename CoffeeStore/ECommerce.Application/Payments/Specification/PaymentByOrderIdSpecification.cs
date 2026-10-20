using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities.Payments;

namespace ECommerce.Application.Payments.Specification;

public class PaymentByOrderIdSpecification : BaseSpecification<Payment>
{
    public PaymentByOrderIdSpecification(Guid orderId)
        : base(p => p.OrderId == orderId)
    {
        AddInclude(p => p.Order);
    }
}
