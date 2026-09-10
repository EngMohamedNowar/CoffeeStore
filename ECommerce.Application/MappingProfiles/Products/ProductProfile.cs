using AutoMapper;
using CoffeeStore.Application.DTOs.Products;
using CoffeeStore.Domain.Entities.Enums;
using CoffeeStore.Domain.Entities.Products;

namespace CoffeeStore.Application.MappingProfiles.Products;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(
                dest => dest.RoastLevel,
                opt => opt.MapFrom(src => src.RoastLevel.ToString())
            )
            .ForMember(
                dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name)
            );

        CreateMap<ProductDto, Product>()
            .ForMember(
                dest => dest.RoastLevel,
                opt => opt.MapFrom(src => Enum.Parse<RoastLevel>(src.RoastLevel))
            )
            .ForMember(
                dest => dest.Category,
                opt => opt.Ignore()
            )
            .ForMember(
                dest => dest.CategoryId,
                opt => opt.Ignore()
            )
            .ForMember(
                dest => dest.Variants,
                opt => opt.Ignore()
            );
    }
}