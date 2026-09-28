using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities.Payments;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public decimal Amount { get; set; }
    public string? TransactionReference { get; set; }
    public DateTime? PaidAt { get; set; }
}