namespace CoffeeStore.Application.DTOs.Products;

public class CreateProductVariantDto
{
    public int WeightInGrams { get; set; }
    public string GrindType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Sku { get; set; } = string.Empty;
}