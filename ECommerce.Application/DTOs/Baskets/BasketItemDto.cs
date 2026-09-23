using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ECommerce.Application.DTOs.Baskets
{
    public class BasketItemDto
    {
        [Required(ErrorMessage = "Product Id is Required")]
        public Guid ProductId { get; init; }

        [Required(ErrorMessage = "Product Name is Required")]
        public string ProductName { get; init; } = default!;

        public string PictureUrl { get; init; } = default!;

        [Range(1, double.MaxValue, ErrorMessage = "Price Must Be A Positive Number")]
        public decimal UnitPrice { get; init; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity Must Be At Least One")]
        public int Quantity { get; init; }

        [JsonConstructor]
        public BasketItemDto(
            Guid productId,
            string productName,
            string pictureUrl,
            decimal unitPrice,
            int quantity)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = pictureUrl;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}
