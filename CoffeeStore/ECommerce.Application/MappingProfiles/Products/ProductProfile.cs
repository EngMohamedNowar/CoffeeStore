using AutoMapper;
using ECommerce.Application.DTOs.Products;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Products;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.MappingProfiles.Products;

public class ProductProfile : Profile
{
    public ProductProfile()
        : this(null)
    {
    }

    public ProductProfile(IConfiguration? configuration)
    {
        var baseUrl = (configuration?["BaseUrl"] ?? string.Empty).TrimEnd('/');

        CreateMap<Product, ProductDto>()
            .ForMember(
                dest => dest.RoastLevel,
                opt => opt.MapFrom(src => src.RoastLevel.ToString()))
            .ForMember(
                dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(
                dest => dest.MinPrice,
                opt => opt.MapFrom(src => src.Variants.Count == 0
                    ? 0m
                    : src.Variants.Min(v => v.Price)))
            .ForMember(
                dest => dest.ImageUrl,
                opt => opt.MapFrom(src => BuildImageUrl(baseUrl, src.ImageUrl)));

        CreateMap<ProductVariant, ProductVariantDto>();

        CreateMap<ProductDto, Product>()
            .ForMember(
                dest => dest.RoastLevel,
                opt => opt.MapFrom(src => Enum.Parse<RoastLevel>(src.RoastLevel)))
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.Variants, opt => opt.Ignore());
    }

    private static string? BuildImageUrl(string baseUrl, string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return null;
        }

        return string.IsNullOrEmpty(baseUrl)
            ? imageUrl
            : $"{baseUrl}/{imageUrl.TrimStart('/')}";
    }
}