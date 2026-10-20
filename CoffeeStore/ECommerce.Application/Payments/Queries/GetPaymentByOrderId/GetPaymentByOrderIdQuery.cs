using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Payments.Dtos;

namespace ECommerce.Application.Payments.Queries.GetPaymentByOrderId;

public sealed record GetPaymentByOrderIdQuery(Guid OrderId, string Email)
    : IQuery<Result<PaymentResponse>>;
