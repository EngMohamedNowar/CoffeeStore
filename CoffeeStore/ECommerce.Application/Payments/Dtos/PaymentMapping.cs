using ECommerce.Domain.Entities.Payments;

namespace ECommerce.Application.Payments.Dtos;

public static class PaymentMapping
{
    public static PaymentResponse ToResponse(Payment payment, string? clientSecret = null)
    {
        ArgumentNullException.ThrowIfNull(payment);

        return new PaymentResponse(
            payment.Id,
            payment.OrderId,
            (int)payment.Method,
            (int)payment.Status,
            payment.Amount,
            payment.TransactionReference,
            payment.PaidAt is null
                ? null
                : new DateTimeOffset(DateTime.SpecifyKind(payment.PaidAt.Value, DateTimeKind.Utc)),
            clientSecret);
    }
}
