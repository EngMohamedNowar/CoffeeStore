using CoffeeStore.Domain.Entities.Categories;
using CoffeeStore.Domain.Entities.Enums;
using ECommerce.Domain.Common;

namespace CoffeeStore.Domain.Entities.Products;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public RoastLevel RoastLevel { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}