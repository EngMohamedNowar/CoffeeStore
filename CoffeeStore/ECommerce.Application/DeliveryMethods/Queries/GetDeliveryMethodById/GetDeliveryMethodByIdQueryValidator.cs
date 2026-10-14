using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace ECommerce.Application.DeliveryMethods.Queries.GetDeliveryMethodById;

public class GetDeliveryMethodByIdQueryValidator : AbstractValidator<GetDeliveryMethodByIdQuery>
{
        public GetDeliveryMethodByIdQueryValidator()
        {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode("DeliveryMethod.Id.Required")
        .WithMessage("Delivery Method Id is Required");
        }
    
}
