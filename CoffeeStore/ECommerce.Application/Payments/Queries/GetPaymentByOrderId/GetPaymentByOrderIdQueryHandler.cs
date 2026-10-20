using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Payments.Dtos;
using ECommerce.Application.Payments.Specification;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Errors;
using FluentValidation;

namespace ECommerce.Application.Payments.Queries.GetPaymentByOrderId;

internal class GetPaymentByOrderIdQueryHandler(
    IReadRepository<Payment> repository,
    IValidator<GetPaymentByOrderIdQuery> validator)
    : IQueryHandler<GetPaymentByOrderIdQuery, Result<PaymentResponse>>
{
    public async Task<Result<PaymentResponse>> HandleAsync(
        GetPaymentByOrderIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PaymentResponse>.Fail(validation.Errors
                .Select(e => Error.Validation(e.ErrorCode, e.ErrorMessage))
                .ToArray());
        }

        var payments = await repository.GetAllAsync(
            new PaymentByOrderIdSpecification(query.OrderId),
            cancellationToken);

        var payment = payments.FirstOrDefault();

        if (payment is null)
            return Result<PaymentResponse>.Fail(PaymentErrors.NotFound);

        if (!string.Equals(
                payment.Order.UserEmail,
                query.Email.Trim(),
                StringComparison.OrdinalIgnoreCase))
            return Result<PaymentResponse>.Fail(OrderErrors.Forbidden);

        return Result<PaymentResponse>.Ok(PaymentMapping.ToResponse(payment));
    }
}
