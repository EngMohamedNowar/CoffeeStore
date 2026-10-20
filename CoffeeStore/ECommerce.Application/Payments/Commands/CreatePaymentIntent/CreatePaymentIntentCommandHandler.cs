using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Payments.Dtos;
using ECommerce.Application.Payments.Specification;
using ECommerce.Application.Services.Contracts;
using ECommerce.Application.Settings;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Errors;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace ECommerce.Application.Payments.Commands.CreatePaymentIntent;

internal class CreatePaymentIntentCommandHandler(
    IReadRepository<Order> orderRepository,
    IReadRepository<Payment> paymentRepository,
    IUnitOfWork unitOfWork,
    IPaymentService paymentService,
    IOptions<StripeSettings> stripeOptions,
    IValidator<CreatePaymentIntentCommand> validator)
    : IRequestHandler<CreatePaymentIntentCommand, Result<PaymentResponse>>
{
    public async Task<Result<PaymentResponse>> HandleAsync(
        CreatePaymentIntentCommand command,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PaymentResponse>.Fail(validation.Errors
                .Select(e => Error.Validation(e.ErrorCode, e.ErrorMessage))
                .ToArray());
        }

        var order = await orderRepository.GetBySpecAsync(
            new Orders.Specification.OrderByIdSpecification(command.OrderId),
            cancellationToken);

        if (order is null)
            return Result<PaymentResponse>.Fail(OrderErrors.NotFound);

        if (!string.Equals(
                order.UserEmail,
                command.Email.Trim(),
                StringComparison.OrdinalIgnoreCase))
            return Result<PaymentResponse>.Fail(OrderErrors.Forbidden);

        if (order.Status != OrderStatus.Pending)
            return Result<PaymentResponse>.Fail(OrderErrors.InvalidPaymentState);

        var existingPayments = await paymentRepository.GetAllAsync(
            new PaymentByOrderIdSpecification(command.OrderId),
            cancellationToken);

        var existingPayment = existingPayments.FirstOrDefault();

        if (existingPayment is not null && existingPayment.Status == PaymentStatus.Paid)
            return Result<PaymentResponse>.Fail(PaymentErrors.AlreadyPaid);

        var amount = (long)(order.GetTotal() * 100);
        var currency = stripeOptions.Value.Currency;

        PaymentIntentResult intentResult;

        if (existingPayment is not null && !string.IsNullOrEmpty(existingPayment.TransactionReference))
        {
            var updateResult = await paymentService.UpdatePaymentIntentAsync(
                existingPayment.TransactionReference!,
                amount,
                command.OrderId,
                cancellationToken);

            if (updateResult.IsFailure)
                return Result<PaymentResponse>.Fail(updateResult.Errors);

            intentResult = updateResult.Value!;
            existingPayment.Amount = order.GetTotal();
            existingPayment.TransactionReference = intentResult.PaymentIntentId;
            unitOfWork.GetRepository<Payment>().Update(existingPayment);
        }
        else
        {
            var createResult = await paymentService.CreatePaymentIntentAsync(
                amount,
                currency,
                command.OrderId,
                cancellationToken);

            if (createResult.IsFailure)
                return Result<PaymentResponse>.Fail(createResult.Errors);

            intentResult = createResult.Value!;

            existingPayment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Method = PaymentMethod.CreditCard,
                Status = PaymentStatus.Pending,
                Amount = order.GetTotal(),
                TransactionReference = intentResult.PaymentIntentId,
                CreatedAt = DateTime.UtcNow
            };

            unitOfWork.GetRepository<Payment>().Add(existingPayment);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PaymentResponse>.Ok(
            PaymentMapping.ToResponse(existingPayment, intentResult.ClientSecret));
    }
}
