using Microsoft.AspNetCore.Identity;
namespace ECommerce.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
    }
}