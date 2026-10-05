namespace ECommerce.Application.Common;

public static class AddressErrors
{
    public static readonly Error AddressNotFound =
        Error.NotFound(
            "Address.NotFound",
            "Address was not found.");

    public static readonly Error AddressInvalid =
        Error.Validation(
            "Address.Invalid",
            "Address data is not valid.");
}
