namespace CoffeeStore.Application.DTOs.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string RoastLevel { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal MinPrice { get; set; }   // أقل سعر بين الـ Variants — يظهر في صفحة الكتالوج
    public List<ProductVariantDto> Variants { get; set; } = new();
}