using System;

namespace ECommerce.Application.Payments.Dtos;

public sealed record PaymentResponse(
    Guid Id,
    Guid OrderId,
    int Method,
    int Status,
    decimal Amount,
    string? TransactionReference,
    DateTimeOffset? PaidAt,
    string? ClientSecret = null);
