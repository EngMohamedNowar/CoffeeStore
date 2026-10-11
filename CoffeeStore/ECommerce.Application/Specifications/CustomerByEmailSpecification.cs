using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities.Customers;

namespace ECommerce.Application.Specifications;

public class CustomerByEmailSpecification : BaseSpecification<Customer>
{
    public CustomerByEmailSpecification(string email)
        : base(c => c.Email == email)
    {
    }
}
