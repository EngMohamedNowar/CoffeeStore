namespace ECommerce.Domain.Entities.Identity;

public class Address
{
    public int Id { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
}
