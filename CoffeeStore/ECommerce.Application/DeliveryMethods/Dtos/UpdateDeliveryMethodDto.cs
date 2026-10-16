namespace ECommerce.Application.DeliveryMethods.Dtos;

public class UpdateDeliveryMethodDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string EstimatedDeliveryTime { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
    public int DisplayOrder { get; set; }
}
