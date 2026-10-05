using Microsoft.AspNetCore.Identity;
namespace ECommerce.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
