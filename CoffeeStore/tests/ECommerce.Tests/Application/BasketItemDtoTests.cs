using System.ComponentModel.DataAnnotations;
using ECommerce.Application.DTOs.Baskets;
using ECommerce.Domain.Entities.Baskets;

namespace ECommerce.Tests.Application;

public class BasketItemDtoTests
{
    private static List<ValidationResult> Validate(BasketItemDto item)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(item, new ValidationContext(item), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void PictureUrl_IsOptional()
    {
        var item = new BasketItemDto(
            productId: Guid.NewGuid(),
            productName: "Ethiopian Yirgacheffe",
            pictureUrl: null,
            unitPrice: 12.50m,
            quantity: 2);

        var results = Validate(item);

        Assert.Empty(results);
        Assert.Null(item.PictureUrl);
    }

    [Fact]
    public void ProductName_IsStillRequired()
    {
        var item = new BasketItemDto(
            productId: Guid.NewGuid(),
            productName: string.Empty,
            pictureUrl: null,
            unitPrice: 12.50m,
            quantity: 1);

        var results = Validate(item);

        Assert.Contains(results, r => r.ErrorMessage == "Product Name is Required");
    }

    [Fact]
    public void Quantity_MustBeAtLeastOne()
    {
        var item = new BasketItemDto(
            productId: Guid.NewGuid(),
            productName: "Ethiopian Yirgacheffe",
            pictureUrl: null,
            unitPrice: 12.50m,
            quantity: 0);

        var results = Validate(item);

        Assert.Contains(results, r => r.ErrorMessage == "Quantity Must Be At Least One");
    }

    [Fact]
    public void UnitPrice_MustBePositive()
    {
        var item = new BasketItemDto(
            productId: Guid.NewGuid(),
            productName: "Ethiopian Yirgacheffe",
            pictureUrl: null,
            unitPrice: 0m,
            quantity: 1);

        var results = Validate(item);

        Assert.Contains(results, r => r.ErrorMessage == "Price Must Be A Positive Number");
    }

    [Fact]
    public void DomainEntity_AcceptsNullPictureUrl()
    {
        var entity = new BasketItem(
            productId: Guid.NewGuid(),
            productName: "Ethiopian Yirgacheffe",
            pictureUrl: null,
            unitPrice: 12.50m,
            quantity: 2);

        Assert.Null(entity.PictureUrl);
        Assert.Equal(2, entity.Quantity);
    }
}
