using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities.Payments;

namespace ECommerce.Application.Payments.Specification;

public class PaymentByTransactionReferenceSpecification : BaseSpecification<Payment>
{
    public PaymentByTransactionReferenceSpecification(string transactionReference)
        : base(p => p.TransactionReference == transactionReference)
    {
        AddInclude(p => p.Order);
    }
}
