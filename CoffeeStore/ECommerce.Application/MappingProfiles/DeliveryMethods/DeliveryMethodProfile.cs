using AutoMapper;
using ECommerce.Application.DeliveryMethods.Dtos;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.MappingProfiles.DeliveryMethods;

public class DeliveryMethodProfile : Profile
{
    public DeliveryMethodProfile()
    {
        CreateMap<DeliveryMethod, DeliveryMethodResponse>();
    }
}
