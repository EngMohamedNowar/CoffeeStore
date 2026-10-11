using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities.Orders;

public sealed class OrderAddress
{
    private OrderAddress()
    {
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Street { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;

    public static Result<OrderAddress> Create(
        string firstName,
        string lastName,
        string street,
        string city,
        string country)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<OrderAddress>.Failure(OrderErrors.InvalidFirstName);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<OrderAddress>.Failure(OrderErrors.InvalidLastName);

        if (string.IsNullOrWhiteSpace(street))
            return Result<OrderAddress>.Failure(OrderErrors.InvalidStreet);

        if (string.IsNullOrWhiteSpace(city))
            return Result<OrderAddress>.Failure(OrderErrors.InvalidCity);

        if (string.IsNullOrWhiteSpace(country))
            return Result<OrderAddress>.Failure(OrderErrors.InvalidCountry);

        return Result<OrderAddress>.Success(new OrderAddress
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Street = street.Trim(),
            City = city.Trim(),
            Country = country.Trim()
        });
    }
}
