namespace ECommerce.Application.DTOs.Identity;

public class AddressDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? Building { get; set; }
    public string? Notes { get; set; }
    public bool IsDefault { get; set; }

    public string FullName =>
        string.IsNullOrWhiteSpace(LastName) ? FirstName : $"{FirstName} {LastName}";
}
