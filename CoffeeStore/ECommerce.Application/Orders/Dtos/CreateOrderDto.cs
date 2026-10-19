namespace ECommerce.Application.Orders.Dtos;

public class CreateOrderDto
{
    public Guid BasketId { get; set; }
    public Guid DeliveryMethodId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
