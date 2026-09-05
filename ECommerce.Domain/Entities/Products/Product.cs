//using CoffeeStore.Domain.Entities.Categories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities.Products;
//using CoffeeStore.Domain.Entities.Enums;

namespace CoffeeStore.Domain.Entities.Products;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public ProductBrand Brand { get; set; } = default;
    public int BrandId { get; set; }
    public ProductType Type { get; set; } = default;
    public int TypeId { get; set; }
}