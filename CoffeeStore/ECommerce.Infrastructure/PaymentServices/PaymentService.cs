using ECommerce.Application.Payments.Specification;
using ECommerce.Application.Services.Contracts;
using ECommerce.Application.Settings;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Errors;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Events;

namespace ECommerce.Infrastructure.PaymentServices;

public class PaymentService(
    IOptions<StripeSettings> options,
    IUnitOfWork unitOfWork) : IPaymentService
{
    private readonly StripeSettings _settings = options.Value;

    public async Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(
        long amountInSmallestUnit,
        string currency,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new PaymentIntentService();

            var createOptions = new PaymentIntentCreateOptions
            {
                Amount = amountInSmallestUnit,
                Currency = currency,
                Metadata = new Dictionary<string, string>
                {
                    ["orderId"] = orderId.ToString()
                },
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
            };

            var intent = await service.CreateAsync(createOptions, cancellationToken: cancellationToken);

            return Result<PaymentIntentResult>.Ok(new PaymentIntentResult(
                intent.Id,
                intent.ClientSecret,
                intent.Status,
                _settings.PublishableKey));
        }
        catch (StripeException ex)
        {
            return Result<PaymentIntentResult>.Fail(
                Error.Failure(PaymentErrors.StripeError.code, ex.Message));
        }
    }

    public async Task<Result<PaymentIntentResult>> UpdatePaymentIntentAsync(
        string paymentIntentId,
        long amountInSmallestUnit,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new PaymentIntentService();

            var updateOptions = new PaymentIntentUpdateOptions
            {
                Amount = amountInSmallestUnit,
                Metadata = new Dictionary<string, string>
                {
                    ["orderId"] = orderId.ToString()
                }
            };

            var intent = await service.UpdateAsync(
                paymentIntentId, updateOptions, cancellationToken: cancellationToken);

            return Result<PaymentIntentResult>.Ok(new PaymentIntentResult(
                intent.Id,
                intent.ClientSecret,
                intent.Status,
                _settings.PublishableKey));
        }
        catch (StripeException ex)
        {
            return Result<PaymentIntentResult>.Fail(
                Error.Failure(PaymentErrors.StripeError.code, ex.Message));
        }
    }

    public Result<StripeWebhookEvent> ParseWebhook(string jsonPayload, string stripeSignature)
    {
        try
        {
            var @event = EventUtility.ConstructEvent(
                jsonPayload, stripeSignature, _settings.WebhookSecret);

            if (@event.Data.Object is PaymentIntent paymentIntent)
            {
                return Result<StripeWebhookEvent>.Ok(
                    new StripeWebhookEvent(@event.Type, paymentIntent.Id));
            }

            return Result<StripeWebhookEvent>.Fail(OrderErrors.InvalidWebhook);
        }
        catch (StripeException)
        {
            return Result<StripeWebhookEvent>.Fail(OrderErrors.InvalidWebhook);
        }
    }

    public async Task PaymentSucceeded(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        var payments = await unitOfWork.GetRepository<Payment>()
            .GetAllAsync(new PaymentByTransactionReferenceSpecification(paymentIntentId), cancellationToken);

        var payment = payments.FirstOrDefault();
        if (payment is null)
            return;

        payment.Status = PaymentStatus.Paid;
        payment.PaidAt = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;

        payment.Order?.MarkAsPaid();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task PaymentFailed(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        var payments = await unitOfWork.GetRepository<Payment>()
            .GetAllAsync(new PaymentByTransactionReferenceSpecification(paymentIntentId), cancellationToken);

        var payment = payments.FirstOrDefault();
        if (payment is null)
            return;

        payment.Status = PaymentStatus.Failed;
        payment.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
