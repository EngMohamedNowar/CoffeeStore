namespace ECommerce.Domain.Errors;

public static class PaymentErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Payment.NotFound", "Payment was not found.");

    public static readonly Error AlreadyPaid =
        Error.Conflict("Payment.AlreadyPaid", "Payment has already been completed.");

    public static readonly Error StripeError =
        Error.Failure("Payment.StripeError", "Payment provider error occurred.");

    public static readonly Error InvalidOrderId =
        Error.Validation("Payment.InvalidOrderId", "Order id is required.");

    public static readonly Error InvalidSignature =
        Error.Validation("Payment.InvalidSignature", "Webhook signature is required.");

    public static readonly Error InvalidPayload =
        Error.Validation("Payment.InvalidPayload", "Webhook payload is required.");
}
