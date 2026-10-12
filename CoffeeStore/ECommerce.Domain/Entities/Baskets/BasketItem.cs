using System.Text.Json.Serialization;

namespace ECommerce.Domain.Entities.Baskets;

public class BasketItem
{
    public Guid ProductVariantId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public string? PictureUrl { get; private set; }

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    [JsonConstructor]
    public BasketItem(Guid productVariantId, string productName, string? pictureUrl, decimal unitPrice, int quantity)
    {
        ProductVariantId = productVariantId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
