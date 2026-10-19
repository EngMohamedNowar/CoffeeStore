namespace ECommerce.Application.DeliveryMethods.Dtos;

public class CreateDeliveryMethodDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string EstimatedDeliveryTime { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int DisplayOrder { get; set; }
}
